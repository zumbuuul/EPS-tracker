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
    public class BrojilaPage : ListPage
    {
        private readonly BrojiloService brojiloService;
        private readonly SearchEntry searchEntry;
        private readonly ComboBoxText tipFilter;
        private IList<BrojiloListDto> brojila;
        private bool hasTriedInitialLoad;

        public BrojilaPage(BrojiloService brojiloService, Action<string> showStatus)
            : base("Brojila", "Serijski brojevi, tipovi brojila, zamene i tehnicke osobine.", showStatus)
        {
            this.brojiloService = brojiloService;
            brojila = new List<BrojiloListDto>();

            searchEntry = ViewFactory.Search("Pretraga brojila");
            searchEntry.Changed += (sender, args) => ApplyFilters();

            tipFilter = ViewFactory.Combo("Svi tipovi", "JEDNOFAZNO", "TROFAZNO", "PAMETNO", "MEHANICKO", "RASVETNO");
            tipFilter.Changed += (sender, args) => ApplyFilters();

            AddFilter(searchEntry);
            AddFilter(tipFilter);

            AddAction("Dodaj", "list-add", "Novo brojilo", (sender, args) =>
                DodajBrojilo());
            AddAction("Izmeni", "document-edit", "Izmena odabranog brojila", (sender, args) =>
                IzmeniBrojilo());
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog brojila", (sender, args) =>
                ObrisiBrojilo());
            AddAction("Merenja", "accessories-calculator", "Merenja za odabrano brojilo", (sender, args) =>
                PrikaziMerenjaZaBrojilo());
            AddAction("Kvarovi", "dialog-warning", "Kvarovi za odabrano brojilo", (sender, args) =>
                PrikaziKvaroveZaBrojilo());
            AddAction("Osvezi", "view-refresh", "Osvezi brojila", (sender, args) =>
                UcitajBrojila());

            SetTable(
                "Serijski broj",
                "Tipovi",
                "Status",
                "Lokacija",
                "Potrosaci",
                "Datum instalacije",
                "Poslednja zamena",
                "Koeficijent");

            GLib.Idle.Add(() =>
            {
                UcitajBrojilaKadaSeStranicaPrikaze();
                return false;
            });
        }

        private void UcitajBrojilaKadaSeStranicaPrikaze()
        {
            if (hasTriedInitialLoad)
            {
                return;
            }

            hasTriedInitialLoad = true;
            UcitajBrojila();
        }

        private async void UcitajBrojila()
        {
            if (!EnsureService("Brojila nisu ucitana jer BrojiloService nije konfigurisan."))
            {
                ReplaceRows(new List<string[]>());
                return;
            }

            try
            {
                Report("Ucitavanje brojila...");
                var ucitanaBrojila = await brojiloService.VratiBrojila();

                GLib.Idle.Add(() =>
                {
                    brojila = ucitanaBrojila;
                    ApplyFilters();
                    Report("Ucitanih brojila: " + brojila.Count);
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
            var selectedTip = tipFilter.ActiveText;

            var filtered = brojila.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered.Where(x =>
                    Contains(x.SerijskiBroj, query)
                    || Contains(x.Status, query)
                    || Contains(x.Lokacija, query)
                    || x.TipoviBrojila.Any(tip => Contains(tip, query)));
            }

            if (!string.IsNullOrWhiteSpace(selectedTip) && selectedTip != "Svi tipovi")
            {
                filtered = filtered.Where(x => x.TipoviBrojila.Contains(selectedTip));
            }

            var displayedRows = ReplaceRows(filtered.Select(ToRow).ToList());

            if (brojila.Count > 0)
            {
                Report("Ucitanih brojila: " + brojila.Count + ", prikazano: " + displayedRows);
            }
        }

        private async void DodajBrojilo()
        {
            var dialog = new BrojiloDialog(DialogParent);

            while (true)
            {
                dialog.ShowAll();
                var response = (ResponseType)dialog.Run();

                if (response != ResponseType.Ok)
                {
                    dialog.Destroy();
                    return;
                }

                if (!EnsureService("Brojilo nije sacuvano jer BrojiloService nije konfigurisan."))
                {
                    dialog.ShowError("BrojiloService nije konfigurisan. Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
                    continue;
                }

                try
                {
                    dialog.ClearError();
                    var dto = dialog.ToSaveDto();
                    await brojiloService.DodajBrojilo(dto);
                    UcitajBrojila();
                    Report("Brojilo je dodato. Serijski broj: " + dto.SerijskiBroj);
                    dialog.Destroy();
                    return;
                }
                catch (Exception ex)
                {
                    dialog.ShowError(ex.Message);
                    Report("Brojilo nije dodato: " + ex.Message);
                }
            }
        }

        private async void IzmeniBrojilo()
        {
            if (!EnsureService("Brojilo ne moze biti ucitano za izmenu jer BrojiloService nije konfigurisan."))
            {
                return;
            }

            var serijskiBroj = SelectedSerijskiBroj();

            if (string.IsNullOrWhiteSpace(serijskiBroj))
            {
                Report("Izaberi brojilo za izmenu.");
                return;
            }

            try
            {
                var brojilo = await brojiloService.VratiBrojilo(serijskiBroj);
                var dialog = new BrojiloDialog(DialogParent, brojilo);

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
                        await brojiloService.IzmeniBrojilo(dialog.ToSaveDto());
                        dialog.Destroy();
                        UcitajBrojila();
                        Report("Brojilo je izmenjeno.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        dialog.ShowError(ex.Message);
                        Report("Brojilo nije izmenjeno: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private async void ObrisiBrojilo()
        {
            if (!EnsureService("Brojilo ne moze biti obrisano jer BrojiloService nije konfigurisan."))
            {
                return;
            }

            var serijskiBroj = SelectedSerijskiBroj();

            if (string.IsNullOrWhiteSpace(serijskiBroj))
            {
                Report("Izaberi brojilo za brisanje.");
                return;
            }

            try
            {
                await brojiloService.ObrisiBrojilo(serijskiBroj);
                UcitajBrojila();
                Report("Brojilo je obrisano.");
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private async void PrikaziMerenjaZaBrojilo()
        {
            if (!EnsureService("Merenja ne mogu biti ucitana jer BrojiloService nije konfigurisan."))
            {
                return;
            }

            var serijskiBroj = SelectedSerijskiBroj();

            if (string.IsNullOrWhiteSpace(serijskiBroj))
            {
                Report("Izaberi brojilo.");
                return;
            }

            try
            {
                var merenja = await brojiloService.VratiMerenjaZaBrojilo(serijskiBroj);
                var dialog = new BrojiloMerenjaDialog(DialogParent, serijskiBroj, merenja);

                dialog.ShowAll();
                dialog.Run();
                dialog.Destroy();

                Report(merenja.Count == 0
                    ? "Brojilo nema evidentirana merenja."
                    : "Prikazana merenja brojila: " + merenja.Count);
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private async void PrikaziKvaroveZaBrojilo()
        {
            if (!EnsureService("Kvarovi ne mogu biti ucitani jer BrojiloService nije konfigurisan."))
            {
                return;
            }

            var serijskiBroj = SelectedSerijskiBroj();

            if (string.IsNullOrWhiteSpace(serijskiBroj))
            {
                Report("Izaberi brojilo.");
                return;
            }

            try
            {
                var kvarovi = await brojiloService.VratiKvaroveZaBrojilo(serijskiBroj);
                var dialog = new BrojiloKvaroviDialog(DialogParent, serijskiBroj, kvarovi);

                dialog.ShowAll();
                dialog.Run();
                dialog.Destroy();

                Report(kvarovi.Count == 0
                    ? "Brojilo nema evidentirane kvarove."
                    : "Prikazani kvarovi brojila: " + kvarovi.Count);
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private string SelectedSerijskiBroj()
        {
            return SelectedValue(0);
        }

        private bool EnsureService(string message)
        {
            if (brojiloService != null)
            {
                return true;
            }

            Report(message + " Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
            return false;
        }

        private static string[] ToRow(BrojiloListDto brojilo)
        {
            return new[]
            {
                brojilo.SerijskiBroj ?? string.Empty,
                string.Join(", ", brojilo.TipoviBrojila),
                brojilo.Status ?? string.Empty,
                brojilo.Lokacija ?? string.Empty,
                brojilo.BrojPotrosaca.ToString(CultureInfo.InvariantCulture),
                brojilo.DatumInstalacije.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                brojilo.PoslednjiDatumZamene.HasValue
                    ? brojilo.PoslednjiDatumZamene.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    : string.Empty,
                brojilo.KoeficijentMnozenja.HasValue
                    ? brojilo.KoeficijentMnozenja.Value.ToString(CultureInfo.InvariantCulture)
                    : string.Empty
            };
        }

        private static bool Contains(string value, string query)
        {
            return value != null
                && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
