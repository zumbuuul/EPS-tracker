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
    public class StanjeMrezePage : ListPage
    {
        private readonly StanjeService stanjeService;
        private readonly SearchEntry searchEntry;
        private IList<StanjeListDto> stanja;
        private bool hasTriedInitialLoad;

        public StanjeMrezePage(StanjeService stanjeService, Action<string> showStatus)
            : base("Stanje mreze", "Proizvodnja, distribucija, gubici i potrosnja po stanici.", showStatus)
        {
            this.stanjeService = stanjeService;
            stanja = new List<StanjeListDto>();

            searchEntry = ViewFactory.Search("Pretraga stanice");
            searchEntry.Changed += (sender, args) => ApplyFilters();

            AddFilter(searchEntry);

            AddAction("Dodaj", "list-add", "Novo stanje mreze", (sender, args) =>
                DodajStanje());
            AddAction("Izmeni", "document-edit", "Izmena odabranog stanja", (sender, args) =>
                IzmeniStanje());
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog stanja", (sender, args) =>
                ObrisiStanje());
            AddAction("Osvezi", "view-refresh", "Osvezi stanje mreze", (sender, args) =>
                UcitajStanja());

            SetTable(
                "ID",
                "Datum i vreme",
                "Lokacija",
                "Transformator",
                "Naponski nivo",
                "Proizvedena",
                "Distribuirana",
                "Gubici",
                "Ukupna potrosnja",
                "Status");

            GLib.Idle.Add(() =>
            {
                UcitajStanjaKadaSeStranicaPrikaze();
                return false;
            });
        }

        private void UcitajStanjaKadaSeStranicaPrikaze()
        {
            if (hasTriedInitialLoad)
            {
                return;
            }

            hasTriedInitialLoad = true;
            UcitajStanja();
        }

        private async void UcitajStanja()
        {
            if (!EnsureService("Stanje mreze nije ucitano jer StanjeService nije konfigurisan."))
            {
                ReplaceRows(new List<string[]>());
                return;
            }

            try
            {
                Report("Ucitavanje stanja mreze...");
                var ucitanaStanja = await stanjeService.VratiStanja();

                GLib.Idle.Add(() =>
                {
                    stanja = ucitanaStanja;
                    ApplyFilters();
                    Report("Ucitanih stanja mreze: " + stanja.Count);
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
            var filtered = stanja.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered.Where(x =>
                    Contains(x.Lokacija, query)
                    || Contains(x.Transformator, query)
                    || Contains(x.Status, query)
                    || x.Id.ToString(CultureInfo.InvariantCulture).Contains(query));
            }

            var displayedRows = ReplaceRows(filtered.Select(ToRow).ToList());

            if (stanja.Count > 0)
            {
                Report("Ucitanih stanja mreze: " + stanja.Count + ", prikazano: " + displayedRows);
            }
        }

        private async void DodajStanje()
        {
            var dialog = new StanjeDialog(DialogParent);

            while (true)
            {
                dialog.ShowAll();
                var response = (ResponseType)dialog.Run();

                if (response != ResponseType.Ok)
                {
                    dialog.Destroy();
                    return;
                }

                if (!EnsureService("Stanje mreze nije sacuvano jer StanjeService nije konfigurisan."))
                {
                    dialog.ShowError("StanjeService nije konfigurisan. Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
                    continue;
                }

                try
                {
                    dialog.ClearError();
                    var id = await stanjeService.DodajStanje(dialog.ToSaveDto());
                    UcitajStanja();
                    Report("Stanje mreze je dodato. ID: " + id);
                    dialog.Destroy();
                    return;
                }
                catch (Exception ex)
                {
                    dialog.ShowError(ex.Message);
                    Report("Stanje mreze nije dodato: " + ex.Message);
                }
            }
        }

        private async void IzmeniStanje()
        {
            if (!EnsureService("Stanje mreze ne moze biti ucitano za izmenu jer StanjeService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedStanjeId();

            if (!id.HasValue)
            {
                Report("Izaberi stanje mreze za izmenu.");
                return;
            }

            try
            {
                var stanje = await stanjeService.VratiStanje(id.Value);
                var dialog = new StanjeDialog(DialogParent, stanje);

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
                        await stanjeService.IzmeniStanje(dialog.ToSaveDto());
                        dialog.Destroy();
                        UcitajStanja();
                        Report("Stanje mreze je izmenjeno.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        dialog.ShowError(ex.Message);
                        Report("Stanje mreze nije izmenjeno: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private async void ObrisiStanje()
        {
            if (!EnsureService("Stanje mreze ne moze biti obrisano jer StanjeService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedStanjeId();

            if (!id.HasValue)
            {
                Report("Izaberi stanje mreze za brisanje.");
                return;
            }

            try
            {
                await stanjeService.ObrisiStanje(id.Value);
                UcitajStanja();
                Report("Stanje mreze je obrisano.");
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private long? SelectedStanjeId()
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
            if (stanjeService != null)
            {
                return true;
            }

            Report(message + " Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
            return false;
        }

        private static string[] ToRow(StanjeListDto stanje)
        {
            return new[]
            {
                stanje.Id.ToString(CultureInfo.InvariantCulture),
                stanje.DatumIVreme.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                stanje.Lokacija ?? string.Empty,
                stanje.Transformator ?? string.Empty,
                stanje.NaponskiNivo.ToString(CultureInfo.InvariantCulture),
                stanje.ProizvedenaEnergija.ToString(CultureInfo.InvariantCulture),
                stanje.DistribuiranaEnergija.ToString(CultureInfo.InvariantCulture),
                stanje.Gubitak.ToString(CultureInfo.InvariantCulture),
                stanje.UkupnaPotrosnja.ToString(CultureInfo.InvariantCulture),
                stanje.Status ?? string.Empty
            };
        }

        private static bool Contains(string value, string query)
        {
            return value != null
                && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
