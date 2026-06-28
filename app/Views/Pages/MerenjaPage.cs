using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using app.DTO;
using app.Services;
using app.Views.Dialogs;
using app.Views.Ui;
using Gtk;

namespace app.Views.Pages
{
    public class MerenjaPage : ListPage
    {
        private readonly MerenjeService merenjeService;
        private readonly RacunService racunService;
        private readonly SearchEntry searchEntry;
        private readonly ComboBoxText izvorFilter;
        private IList<MerenjeListDto> merenja;
        private bool hasTriedInitialLoad;

        public MerenjaPage(MerenjeService merenjeService, RacunService racunService, Action<string> showStatus)
            : base("Merenja", "Stvarna potrosnja, izvor ocitavanja i validacija.", showStatus)
        {
            this.merenjeService = merenjeService;
            this.racunService = racunService;
            merenja = new List<MerenjeListDto>();

            searchEntry = ViewFactory.Search("Pretraga merenja");
            searchEntry.Changed += (sender, args) => ApplyFilters();

            izvorFilter = ViewFactory.Combo("Svi izvori", "RUCNO_OCITAVANJE", "PAMETNO_BROJILO");
            izvorFilter.Changed += (sender, args) => ApplyFilters();

            AddFilter(searchEntry);
            AddFilter(izvorFilter);

            AddAction("Dodaj", "list-add", "Novo merenje", (sender, args) =>
                DodajMerenje());
            AddAction("Izmeni", "document-edit", "Izmena odabranog merenja", (sender, args) =>
                IzmeniMerenje());
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog merenja", (sender, args) =>
                ObrisiMerenje());
            AddAction("Racun", "x-office-spreadsheet", "Prikazi racun za odabrano merenje", (sender, args) =>
                PrikaziRacunZaMerenje());
            AddAction("Osvezi", "view-refresh", "Osvezi merenja", (sender, args) =>
                UcitajMerenja());

            SetTable(
                "ID",
                "Brojilo",
                "Datum i vreme",
                "Aktivna kWh",
                "Reaktivna kWh",
                "Snaga",
                "Napon",
                "Struja",
                "Tip",
                "Izvor",
                "Validirano");

            GLib.Idle.Add(() =>
            {
                UcitajMerenjaKadaSeStranicaPrikaze();
                return false;
            });
        }

        private void UcitajMerenjaKadaSeStranicaPrikaze()
        {
            if (hasTriedInitialLoad)
            {
                return;
            }

            hasTriedInitialLoad = true;
            UcitajMerenja();
        }

        private async void UcitajMerenja()
        {
            if (!EnsureService("Merenja nisu ucitana jer MerenjeService nije konfigurisan."))
            {
                ReplaceRows(new List<string[]>());
                return;
            }

            try
            {
                Report("Ucitavanje merenja...");
                var ucitanaMerenja = await merenjeService.VratiMerenja();

                GLib.Idle.Add(() =>
                {
                    merenja = ucitanaMerenja;
                    ApplyFilters();
                    Report("Ucitanih merenja: " + merenja.Count);
                    return false;
                });
            }
            catch (Exception ex)
            {
                GLib.Idle.Add(() =>
                {
                    Report(ex.Message);
                    return false;
                });
            }
        }

        private void ApplyFilters()
        {
            var query = searchEntry.Text ?? string.Empty;
            var selectedIzvor = izvorFilter.ActiveText;

            var filtered = merenja.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered.Where(x =>
                    Contains(x.SerijskiBroj, query)
                    || Contains(x.TipMerenja, query)
                    || x.Id.ToString(CultureInfo.InvariantCulture).Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(selectedIzvor) && selectedIzvor != "Svi izvori")
            {
                filtered = filtered.Where(x => x.TipIzvora == selectedIzvor);
            }

            var displayedRows = ReplaceRows(filtered.Select(ToRow).ToList());

            if (merenja.Count > 0)
            {
                Report("Ucitanih merenja: " + merenja.Count + ", prikazano: " + displayedRows);
            }
        }

        private async void DodajMerenje()
        {
            var dialog = new MerenjeDialog(DialogParent);

            while (true)
            {
                dialog.ShowAll();
                var response = (ResponseType)dialog.Run();

                if (response != ResponseType.Ok)
                {
                    dialog.Destroy();
                    return;
                }

                if (!EnsureService("Merenje nije sacuvano jer MerenjeService nije konfigurisan."))
                {
                    dialog.ShowError("MerenjeService nije konfigurisan. Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
                    continue;
                }

                try
                {
                    dialog.ClearError();
                    var id = await merenjeService.DodajMerenje(dialog.ToSaveDto());
                    UcitajMerenja();
                    Report("Merenje je dodato. ID: " + id);
                    dialog.Destroy();
                    return;
                }
                catch (Exception ex)
                {
                    dialog.ShowError(ex.Message);
                    Report("Merenje nije dodato: " + ex.Message);
                }
            }
        }

        private async void IzmeniMerenje()
        {
            if (!EnsureService("Merenje ne moze biti ucitano za izmenu jer MerenjeService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedMerenjeId();

            if (!id.HasValue)
            {
                Report("Izaberi merenje za izmenu.");
                return;
            }

            try
            {
                var merenje = await merenjeService.VratiMerenje(id.Value);
                var dialog = new MerenjeDialog(DialogParent, merenje);

                while (true)
                {
                    dialog.ShowAll();
                    var response = (ResponseType)dialog.Run();

                    if (response != ResponseType.Ok)
                    {
                        dialog.Destroy();
                        return;
                    }

                    try
                    {
                        dialog.ClearError();
                        await merenjeService.IzmeniMerenje(dialog.ToSaveDto());
                        dialog.Destroy();
                        UcitajMerenja();
                        Report("Merenje je izmenjeno.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        dialog.ShowError(ex.Message);
                        Report("Merenje nije izmenjeno: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private async void ObrisiMerenje()
        {
            if (!EnsureService("Merenje ne moze biti obrisano jer MerenjeService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedMerenjeId();

            if (!id.HasValue)
            {
                Report("Izaberi merenje za brisanje.");
                return;
            }

            try
            {
                await merenjeService.ObrisiMerenje(id.Value);
                UcitajMerenja();
                Report("Merenje je obrisano.");
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private async void PrikaziRacunZaMerenje()
        {
            if (!EnsureRacunService("Racun ne moze biti ucitan jer RacunService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedMerenjeId();

            if (!id.HasValue)
            {
                Report("Izaberi merenje.");
                return;
            }

            try
            {
                var racun = await racunService.VratiRacunZaMerenje(id.Value);
                var dialog = new MerenjeRacunDialog(DialogParent, id.Value, racun);

                dialog.ShowAll();
                dialog.Run();
                dialog.Destroy();

                Report(racun == null
                    ? "Merenje nema generisan racun."
                    : "Prikazan racun: " + racun.BrojRacuna);
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private long? SelectedMerenjeId()
        {
            var selected = SelectedValue(0);

            if (long.TryParse(selected, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                return id;
            }

            return null;
        }

        private bool EnsureService(string message)
        {
            if (merenjeService != null)
            {
                return true;
            }

            Report(message + " Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
            return false;
        }

        private bool EnsureRacunService(string message)
        {
            if (racunService != null)
            {
                return true;
            }

            Report(message + " Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
            return false;
        }

        private static string[] ToRow(MerenjeListDto merenje)
        {
            return new[]
            {
                merenje.Id.ToString(CultureInfo.InvariantCulture),
                merenje.SerijskiBroj ?? string.Empty,
                merenje.DatumVremeMerenja.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                FormatDecimal(merenje.PotrosnjaAktivna),
                FormatDecimal(merenje.PotrosnjaReaktivna),
                FormatDecimal(merenje.Snaga),
                FormatDecimal(merenje.Napon),
                FormatDecimal(merenje.Struja),
                merenje.TipMerenja ?? string.Empty,
                merenje.TipIzvora ?? string.Empty,
                merenje.IsValidirano ?? string.Empty
            };
        }

        private static string FormatDecimal(decimal? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        private static bool Contains(string value, string query)
        {
            return value != null
                && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
