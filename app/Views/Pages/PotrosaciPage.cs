using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using app.DTO;
using app.Entities.Enums;
using app.Services;
using app.Views.Dialogs;
using app.Views.Ui;
using Gtk;

namespace app.Views.Pages
{
    public class PotrosaciPage : ListPage
    {
        private readonly PotrosacService potrosacService;
        private readonly SearchEntry searchEntry;
        private readonly ComboBoxText tipFilter;
        private IList<PotrosacListDto> potrosaci;
        private bool hasTriedInitialLoad;

        public PotrosaciPage(PotrosacService potrosacService, Action<string> showStatus)
            : base("Potrosaci", "Domacinstva, firme, institucije i javna rasveta.", showStatus)
        {
            this.potrosacService = potrosacService;
            potrosaci = new List<PotrosacListDto>();

            searchEntry = ViewFactory.Search("Pretraga potrosaca");
            searchEntry.Changed += (sender, args) => ApplyFilters();

            tipFilter = ViewFactory.Combo("Svi tipovi", "DOMACINSTVO", "FIRMA", "INSTITUCIJA", "JAVNA_RASVETA");
            tipFilter.Changed += (sender, args) => ApplyFilters();

            AddFilter(searchEntry);
            AddFilter(tipFilter);

            AddAction("Dodaj", "list-add", "Novi potrosac", (sender, args) =>
                DodajPotrosaca());
            AddAction("Izmeni", "document-edit", "Izmena odabranog potrosaca", (sender, args) =>
                IzmeniPotrosaca());
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog potrosaca", (sender, args) =>
                ObrisiPotrosaca());
            AddAction("Povezi", "insert-link", "Povezi potrosaca i brojilo", (sender, args) =>
                PoveziPotrosacaIBrojilo());
            AddAction("Raskini", "edit-cut", "Raskini vezu potrosaca i brojila", (sender, args) =>
                RaskiniVezuPotrosacBrojilo());
            AddAction("Brojila", "view-list-symbolic", "Prikazi brojila potrosaca", (sender, args) =>
                PrikaziBrojilaZaPotrosaca());
            AddAction("Osvezi", "view-refresh", "Osvezi potrosace", (sender, args) =>
                UcitajPotrosace(true));

            SetTable(
                "ID",
                "Tip",
                "Ime/Naziv",
                "Grad",
                "Telefon",
                "Email",
                "Status",
                "Tarifa",
                "Brojila");

            GLib.Idle.Add(() =>
            {
                UcitajPotrosaceKadaSeStranicaPrikaze();
                return false;
            });
        }

        private void UcitajPotrosaceKadaSeStranicaPrikaze()
        {
            if (hasTriedInitialLoad)
            {
                return;
            }

            hasTriedInitialLoad = true;
            UcitajPotrosace(true);
        }

        private void UcitajPotrosace(bool force = false)
        {
            if (!force && potrosaci.Count > 0)
            {
                ApplyFilters();
                return;
            }

            if (!EnsureService("Potrosaci nisu ucitani jer PotrosacService nije konfigurisan."))
            {
                ReplaceRows(new List<string[]>());
                return;
            }

            TryRun(() =>
            {
                potrosaci = potrosacService.VratiPotrosace();
                ApplyFilters();
                Report("Ucitanih potrosaca: " + potrosaci.Count);
            });
        }

        private void ApplyFilters()
        {
            var query = searchEntry.Text ?? string.Empty;
            var selectedTip = tipFilter.ActiveText;

            var filtered = potrosaci.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered.Where(x =>
                    Contains(x.ImeIliNaziv, query)
                    || Contains(x.Grad, query)
                    || Contains(x.Telefon, query)
                    || Contains(x.Email, query)
                    || x.Id.ToString(CultureInfo.InvariantCulture).Contains(query));
            }

            if (!string.IsNullOrWhiteSpace(selectedTip) && selectedTip != "Svi tipovi")
            {
                filtered = filtered.Where(x => x.Tip.ToString() == selectedTip);
            }

            var displayedRows = ReplaceRows(filtered.Select(ToRow).ToList());

            if (potrosaci.Count > 0)
            {
                Report("Ucitanih potrosaca: " + potrosaci.Count + ", prikazano: " + displayedRows);
            }
        }

        private void DodajPotrosaca()
        {
            var dialog = new PotrosacDialog(DialogParent);

            while (true)
            {
                dialog.ShowAll();
                var response = (ResponseType)dialog.Run();

                if (response != ResponseType.Ok)
                {
                    dialog.Destroy();
                    return;
                }

                if (!EnsureService("Potrosac nije sacuvan jer PotrosacService nije konfigurisan."))
                {
                    dialog.ShowError("PotrosacService nije konfigurisan. Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
                    continue;
                }

                try
                {
                    dialog.ClearError();
                    var id = potrosacService.DodajPotrosaca(dialog.ToSaveDto());
                    UcitajPotrosace(true);
                    Report("Potrosac je dodat. ID: " + id);
                    dialog.Destroy();
                    return;
                }
                catch (Exception ex)
                {
                    dialog.ShowError(ex.Message);
                    Report("Potrosac nije dodat: " + ex.Message);
                }
            }
        }

        private void IzmeniPotrosaca()
        {
            if (!EnsureService("Potrosac ne moze biti ucitan za izmenu jer PotrosacService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedPotrosacId();

            if (!id.HasValue)
            {
                Report("Izaberi potrosaca za izmenu.");
                return;
            }

            TryRun(() =>
            {
                var potrosac = potrosacService.VratiPotrosaca(id.Value);
                var dialog = new PotrosacDialog(DialogParent, potrosac);

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
                        potrosacService.IzmeniPotrosaca(dialog.ToSaveDto());
                        dialog.Destroy();
                        UcitajPotrosace(true);
                        Report("Potrosac je izmenjen.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        dialog.ShowError(ex.Message);
                        Report("Potrosac nije izmenjen: " + ex.Message);
                    }
                }
            });
        }

        private void ObrisiPotrosaca()
        {
            if (!EnsureService("Potrosac ne moze biti obrisan jer PotrosacService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedPotrosacId();

            if (!id.HasValue)
            {
                Report("Izaberi potrosaca za brisanje.");
                return;
            }

            TryRun(() =>
            {
                potrosacService.ObrisiPotrosaca(id.Value);
                UcitajPotrosace(true);
                Report("Potrosac je obrisan.");
            });
        }

        private void PoveziPotrosacaIBrojilo()
        {
            RunLinkOperation((service, dto) => service.PoveziPotrosacaIBrojilo(dto), "Potrosac i brojilo su povezani.");
        }

        private void RaskiniVezuPotrosacBrojilo()
        {
            RunLinkOperation((service, dto) => service.RaskiniVezuPotrosacBrojilo(dto), "Veza potrosaca i brojila je raskinuta.");
        }

        private void RunLinkOperation(Action<PotrosacService, PotrosacBrojiloLinkDto> operation, string successMessage)
        {
            var id = SelectedPotrosacId();

            if (!id.HasValue)
            {
                Report("Izaberi potrosaca.");
                return;
            }

            var dialog = new PotrosacBrojiloLinkDialog(DialogParent, id.Value);

            while (true)
            {
                dialog.ShowAll();
                var response = (ResponseType)dialog.Run();

                if (response != ResponseType.Ok)
                {
                    dialog.Destroy();
                    return;
                }

                if (!EnsureService("Veza nije sacuvana jer PotrosacService nije konfigurisan."))
                {
                    dialog.ShowError("PotrosacService nije konfigurisan. Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
                    continue;
                }

                try
                {
                    dialog.ClearError();
                    operation(potrosacService, dialog.ToDto());
                    UcitajPotrosace(true);
                    Report(successMessage);
                    dialog.Destroy();
                    return;
                }
                catch (Exception ex)
                {
                    dialog.ShowError(ex.Message);
                    Report("Veza nije sacuvana: " + ex.Message);
                }
            }
        }

        private void PrikaziBrojilaZaPotrosaca()
        {
            if (!EnsureService("Brojila ne mogu biti ucitana jer PotrosacService nije konfigurisan."))
            {
                return;
            }

            var id = SelectedPotrosacId();

            if (!id.HasValue)
            {
                Report("Izaberi potrosaca.");
                return;
            }

            TryRun(() =>
            {
                var brojila = potrosacService.VratiBrojilaZaPotrosaca(id.Value);
                var serijskiBrojevi = string.Join(", ", brojila.Select(x => x.SerijskiBroj));

                Report(brojila.Count == 0
                    ? "Potrosac nema povezana brojila."
                    : "Brojila potrosaca: " + serijskiBrojevi);
            });
        }

        private long? SelectedPotrosacId()
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
            if (potrosacService != null)
            {
                return true;
            }

            Report(message + " Podesi EPS_TRACKER_ORACLE_CONNECTION_STRING ili ORACLE_CONNECTION_STRING.");
            return false;
        }

        private bool TryRun(System.Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                Report(ex.Message);
                return false;
            }
        }

        private static string[] ToRow(PotrosacListDto potrosac)
        {
            return new[]
            {
                potrosac.Id.ToString(CultureInfo.InvariantCulture),
                potrosac.Tip.ToString(),
                potrosac.ImeIliNaziv ?? string.Empty,
                potrosac.Grad ?? string.Empty,
                potrosac.Telefon ?? string.Empty,
                potrosac.Email ?? string.Empty,
                potrosac.Status ?? string.Empty,
                potrosac.KategorijaTarife ?? string.Empty,
                potrosac.BrojBrojila.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static bool Contains(string value, string query)
        {
            return value != null
                && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
