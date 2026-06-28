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
    public class RacuniPage : ListPage
    {
        private readonly RacunService racunService;
        private readonly SearchEntry searchEntry;
        private readonly ComboBoxText statusFilter;
        private readonly ComboBoxText nacinPlacanjaFilter;
        private IList<RacunListDto> racuni;
        private bool hasTriedInitialLoad;

        public RacuniPage(RacunService racunService, Action<string> showStatus)
            : base("Racuni", "Racuni generisani iz merenja za potrosaca i brojilo.", showStatus)
        {
            this.racunService = racunService;
            racuni = new List<RacunListDto>();

            searchEntry = ViewFactory.Search("Pretraga racuna");
            searchEntry.Changed += (sender, args) => ApplyFilters();

            statusFilter = ViewFactory.Combo("Svi statusi", "POSLAT", "PLACEN", "KASNJENJE", "STORNIRAN");
            statusFilter.Changed += (sender, args) => ApplyFilters();

            nacinPlacanjaFilter = ViewFactory.Combo("Svi nacini", "UPLATNICA", "VIRMAN", "ONLINE", "TRAJNI_NALOG");
            nacinPlacanjaFilter.Changed += (sender, args) => ApplyFilters();

            AddFilter(searchEntry);
            AddFilter(statusFilter);
            AddFilter(nacinPlacanjaFilter);

            AddAction("Izmeni", "document-edit", "Izmena odabranog racuna", (sender, args) =>
                IzmeniRacun());
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog racuna", (sender, args) =>
                ObrisiRacun());
            AddAction("Osvezi", "view-refresh", "Osvezi racune", (sender, args) =>
                UcitajRacune());

            SetTable(
                "Broj racuna",
                "Potrosac",
                "Brojilo",
                "Period",
                "Ukupna potrosnja",
                "Iznos bez PDV",
                "PDV",
                "Ukupno",
                "Status",
                "Datum izdavanja",
                "Rok placanja",
                "Nacin placanja");

            GLib.Idle.Add(() =>
            {
                UcitajRacuneKadaSeStranicaPrikaze();
                return false;
            });
        }

        private void UcitajRacuneKadaSeStranicaPrikaze()
        {
            if (hasTriedInitialLoad)
            {
                return;
            }

            hasTriedInitialLoad = true;
            UcitajRacune();
        }

        private async void UcitajRacune()
        {
            if (!EnsureService("Racuni nisu ucitani jer RacunService nije konfigurisan."))
            {
                ReplaceRows(new List<string[]>());
                return;
            }

            try
            {
                Report("Ucitavanje racuna...");
                var ucitaniRacuni = await racunService.VratiRacune();

                GLib.Idle.Add(() =>
                {
                    racuni = ucitaniRacuni;
                    ApplyFilters();
                    Report("Ucitanih racuna: " + racuni.Count);
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
            var selectedStatus = statusFilter.ActiveText;
            var selectedNacinPlacanja = nacinPlacanjaFilter.ActiveText;

            var filtered = racuni.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered.Where(x =>
                    Contains(x.BrojRacuna, query)
                    || Contains(x.ImeIliNazivPotrosaca, query)
                    || Contains(x.SerijskiBroj, query)
                    || x.PotrosacId.ToString(CultureInfo.InvariantCulture).Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus) && selectedStatus != "Svi statusi")
            {
                filtered = filtered.Where(x => x.Status == selectedStatus);
            }

            if (!string.IsNullOrWhiteSpace(selectedNacinPlacanja) && selectedNacinPlacanja != "Svi nacini")
            {
                filtered = filtered.Where(x => x.NacinPlacanja == selectedNacinPlacanja);
            }

            var displayedRows = ReplaceRows(filtered.Select(ToRow).ToList());

            if (racuni.Count > 0)
            {
                Report("Ucitanih racuna: " + racuni.Count + ", prikazano: " + displayedRows);
            }
        }

        private async void IzmeniRacun()
        {
            if (!EnsureService("Racun ne moze biti ucitan za izmenu jer RacunService nije konfigurisan."))
            {
                return;
            }

            var brojRacuna = SelectedBrojRacuna();

            if (string.IsNullOrWhiteSpace(brojRacuna))
            {
                Report("Izaberi racun za izmenu.");
                return;
            }

            try
            {
                var racun = await racunService.VratiRacun(brojRacuna);
                var dialog = new RacunDialog(DialogParent, racun);

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
                        await racunService.IzmeniRacun(dialog.ToSaveDto());
                        dialog.Destroy();
                        UcitajRacune();
                        Report("Racun je izmenjen.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        dialog.ShowError(ex.Message);
                        Report("Racun nije izmenjen: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private async void ObrisiRacun()
        {
            if (!EnsureService("Racun ne moze biti obrisan jer RacunService nije konfigurisan."))
            {
                return;
            }

            var brojRacuna = SelectedBrojRacuna();

            if (string.IsNullOrWhiteSpace(brojRacuna))
            {
                Report("Izaberi racun za brisanje.");
                return;
            }

            try
            {
                await racunService.ObrisiRacun(brojRacuna);
                UcitajRacune();
                Report("Racun je obrisan.");
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private string SelectedBrojRacuna()
        {
            return SelectedValue(0);
        }

        private bool EnsureService(string message)
        {
            if (racunService != null)
            {
                return true;
            }

            Report(message + " Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
            return false;
        }

        private static string[] ToRow(RacunListDto racun)
        {
            return new[]
            {
                racun.BrojRacuna ?? string.Empty,
                racun.ImeIliNazivPotrosaca ?? string.Empty,
                racun.SerijskiBroj ?? string.Empty,
                FormatDate(racun.PeriodPotrosnjeOd) + " - " + FormatDate(racun.PeriodPotrosnjeDo),
                FormatDecimal(racun.UkupnaPotrosnja),
                racun.IznosBezPdv.ToString(CultureInfo.InvariantCulture),
                racun.Pdv.ToString(CultureInfo.InvariantCulture),
                racun.UkupanIznos.ToString(CultureInfo.InvariantCulture),
                racun.Status ?? string.Empty,
                FormatDate(racun.DatumIzdavanja),
                FormatDate(racun.RokPlacanja),
                racun.NacinPlacanja ?? string.Empty
            };
        }

        private static string FormatDate(DateTime value)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
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
