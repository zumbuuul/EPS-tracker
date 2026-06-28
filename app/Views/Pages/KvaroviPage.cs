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
    public class KvaroviPage : ListPage
    {
        private readonly KvarService kvarService;
        private readonly SearchEntry searchEntry;
        private readonly ComboBoxText statusFilter;
        private readonly ComboBoxText prioritetFilter;
        private IList<KvarListDto> kvarovi;
        private bool hasTriedInitialLoad;

        public KvaroviPage(KvarService kvarService, Action<string> showStatus)
            : base("Kvarovi", "Prijave, prioriteti, timovi i otklanjanje kvarova.", showStatus)
        {
            this.kvarService = kvarService;
            kvarovi = new List<KvarListDto>();

            searchEntry = ViewFactory.Search("Pretraga kvarova");
            searchEntry.Changed += (sender, args) => ApplyFilters();

            statusFilter = ViewFactory.Combo("Svi statusi", "PRIJAVLJEN", "U_OBRADI", "OTKLONJEN", "ZATVOREN");
            statusFilter.Changed += (sender, args) => ApplyFilters();

            prioritetFilter = ViewFactory.Combo("Svi prioriteti", "NIZAK", "SREDNJI", "VISOK", "KRITICAN");
            prioritetFilter.Changed += (sender, args) => ApplyFilters();

            AddFilter(searchEntry);
            AddFilter(statusFilter);
            AddFilter(prioritetFilter);

            AddAction("Dodaj", "list-add", "Nova prijava kvara", (sender, args) =>
                DodajKvar());
            AddAction("Izmeni", "document-edit", "Izmena odabranog kvara", (sender, args) =>
                IzmeniKvar());
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog kvara", (sender, args) =>
                ObrisiKvar());
            AddAction("Otkloni", "emblem-ok", "Oznaci kvar kao otklonjen", (sender, args) =>
                OtkloniKvar());
            AddAction("Osvezi", "view-refresh", "Osvezi kvarove", (sender, args) =>
                UcitajKvarove());

            SetTable(
                "ID",
                "Tip kvara",
                "Brojilo",
                "Potrosac",
                "Datum prijave",
                "Status",
                "Prioritet",
                "Nadlezni tim",
                "Trajanje",
                "Otklonjeno");

            GLib.Idle.Add(() =>
            {
                UcitajKvaroveKadaSeStranicaPrikaze();
                return false;
            });
        }

        private void UcitajKvaroveKadaSeStranicaPrikaze()
        {
            if (hasTriedInitialLoad)
            {
                return;
            }

            hasTriedInitialLoad = true;
            UcitajKvarove();
        }

        private async void UcitajKvarove()
        {
            if (!EnsureService("Kvarovi nisu ucitani jer KvarService nije konfigurisan."))
            {
                ReplaceRows(new List<string[]>());
                return;
            }

            try
            {
                Report("Ucitavanje kvarova...");
                var ucitaniKvarovi = await kvarService.VratiKvarove();

                GLib.Idle.Add(() =>
                {
                    kvarovi = ucitaniKvarovi;
                    ApplyFilters();
                    Report("Ucitanih kvarova: " + kvarovi.Count);
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
            var selectedPrioritet = prioritetFilter.ActiveText;

            var filtered = kvarovi.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered.Where(x =>
                    Contains(x.TipKvara, query)
                    || Contains(x.SerijskiBroj, query)
                    || Contains(x.ImeIliNazivPotrosaca, query)
                    || Contains(x.NadlezniTim, query)
                    || x.Id.ToString(CultureInfo.InvariantCulture).Contains(query)
                    || x.PotrosacId.ToString(CultureInfo.InvariantCulture).Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(selectedStatus) && selectedStatus != "Svi statusi")
            {
                filtered = filtered.Where(x => x.Status == selectedStatus);
            }

            if (!string.IsNullOrWhiteSpace(selectedPrioritet) && selectedPrioritet != "Svi prioriteti")
            {
                filtered = filtered.Where(x => x.Prioritet == selectedPrioritet);
            }

            var displayedRows = ReplaceRows(filtered.Select(ToRow).ToList());

            if (kvarovi.Count > 0)
            {
                Report("Ucitanih kvarova: " + kvarovi.Count + ", prikazano: " + displayedRows);
            }
        }

        private async void DodajKvar()
        {
            var dialog = new KvarDialog(DialogParent);

            while (true)
            {
                dialog.ShowAll();
                var response = (ResponseType)dialog.Run();

                if (response != ResponseType.Ok)
                {
                    dialog.Destroy();
                    return;
                }

                if (!EnsureService("Kvar nije sacuvan jer KvarService nije konfigurisan."))
                {
                    dialog.ShowError("KvarService nije konfigurisan. Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
                    continue;
                }

                try
                {
                    dialog.ClearError();
                    var id = await kvarService.DodajKvar(dialog.ToSaveDto());
                    UcitajKvarove();
                    Report("Kvar je dodat. ID: " + id);
                    dialog.Destroy();
                    return;
                }
                catch (Exception ex)
                {
                    dialog.ShowError(ex.Message);
                    Report("Kvar nije dodat: " + ex.Message);
                }
            }
        }

        private async void IzmeniKvar()
        {
            if (!EnsureService("Kvar ne moze biti ucitan za izmenu jer KvarService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedKvarId();

            if (!id.HasValue)
            {
                Report("Izaberi kvar za izmenu.");
                return;
            }

            try
            {
                var kvar = await kvarService.VratiKvar(id.Value);
                var dialog = new KvarDialog(DialogParent, kvar);

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
                        await kvarService.IzmeniKvar(dialog.ToSaveDto());
                        dialog.Destroy();
                        UcitajKvarove();
                        Report("Kvar je izmenjen.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        dialog.ShowError(ex.Message);
                        Report("Kvar nije izmenjen: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private async void ObrisiKvar()
        {
            if (!EnsureService("Kvar ne moze biti obrisan jer KvarService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedKvarId();

            if (!id.HasValue)
            {
                Report("Izaberi kvar za brisanje.");
                return;
            }

            try
            {
                await kvarService.ObrisiKvar(id.Value);
                UcitajKvarove();
                Report("Kvar je obrisan.");
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private async void OtkloniKvar()
        {
            if (!EnsureService("Kvar ne moze biti otklonjen jer KvarService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedKvarId();

            if (!id.HasValue)
            {
                Report("Izaberi kvar za otklanjanje.");
                return;
            }

            try
            {
                await kvarService.OtkloniKvar(id.Value);
                UcitajKvarove();
                Report("Kvar je oznacen kao otklonjen.");
            }
            catch (Exception ex)
            {
                Report(ex.Message);
            }
        }

        private long? SelectedKvarId()
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
            if (kvarService != null)
            {
                return true;
            }

            Report(message + " Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
            return false;
        }

        private static string[] ToRow(KvarListDto kvar)
        {
            return new[]
            {
                kvar.Id.ToString(CultureInfo.InvariantCulture),
                kvar.TipKvara ?? string.Empty,
                kvar.SerijskiBroj ?? string.Empty,
                kvar.ImeIliNazivPotrosaca ?? kvar.PotrosacId.ToString(CultureInfo.InvariantCulture),
                kvar.DatumPrijave.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                kvar.Status ?? string.Empty,
                kvar.Prioritet ?? string.Empty,
                kvar.NadlezniTim ?? string.Empty,
                kvar.TrajanjeUSatima.HasValue
                    ? kvar.TrajanjeUSatima.Value.ToString(CultureInfo.InvariantCulture)
                    : string.Empty,
                kvar.DatumOtklanjanja.HasValue
                    ? kvar.DatumOtklanjanja.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)
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
