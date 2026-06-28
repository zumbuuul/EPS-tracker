using System.Globalization;
using app.DTO;
using Gtk;

namespace app.Views.Dialogs
{
    public class MerenjeRacunDialog : Dialog
    {
        private readonly Grid grid;
        private int row;

        public MerenjeRacunDialog(Window parent, long merenjeId, RacunDto racun)
            : base("Racun merenja", parent, DialogFlags.Modal)
        {
            SetDefaultSize(620, 420);
            DestroyWithParent = true;
            ContentArea.BorderWidth = 12;

            grid = new Grid
            {
                ColumnSpacing = 12,
                RowSpacing = 8
            };

            ContentArea.PackStart(grid, true, true, 0);

            AddTitle("Merenje ID: " + merenjeId.ToString(CultureInfo.InvariantCulture));

            if (racun == null)
            {
                AddValue("Status", "Merenje nema generisan racun.");
            }
            else
            {
                AddValue("Broj racuna", racun.BrojRacuna);
                AddValue("Potrosac", racun.ImeIliNazivPotrosaca);
                AddValue("Potrosac ID", racun.PotrosacId.ToString(CultureInfo.InvariantCulture));
                AddValue("Brojilo", racun.SerijskiBroj);
                AddValue("Period", FormatDate(racun.PeriodPotrosnjeOd) + " - " + FormatDate(racun.PeriodPotrosnjeDo));
                AddValue("Ukupna potrosnja", FormatDecimal(racun.UkupnaPotrosnja));
                AddValue("Iznos bez PDV", racun.IznosBezPdv.ToString(CultureInfo.InvariantCulture));
                AddValue("PDV", racun.Pdv.ToString(CultureInfo.InvariantCulture));
                AddValue("Ukupan iznos", racun.UkupanIznos.ToString(CultureInfo.InvariantCulture));
                AddValue("Datum izdavanja", FormatDate(racun.DatumIzdavanja));
                AddValue("Rok placanja", FormatDate(racun.RokPlacanja));
                AddValue("Status", racun.Status);
                AddValue("Nacin placanja", racun.NacinPlacanja);
                AddValue("Komentar", racun.Komentar);
            }

            AddButton("Zatvori", ResponseType.Close);
        }

        private void AddTitle(string text)
        {
            var label = new Label
            {
                Markup = "<b>" + text + "</b>",
                Xalign = 0,
                MarginBottom = 8
            };

            grid.Attach(label, 0, row, 2, 1);
            row++;
        }

        private void AddValue(string labelText, string value)
        {
            var label = new Label(labelText)
            {
                Xalign = 0
            };

            var text = new Label(value ?? string.Empty)
            {
                Xalign = 0,
                LineWrap = true,
                Selectable = true
            };

            grid.Attach(label, 0, row, 1, 1);
            grid.Attach(text, 1, row, 1, 1);
            row++;
        }

        private static string FormatDate(System.DateTime value)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static string FormatDecimal(decimal? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }
    }
}
