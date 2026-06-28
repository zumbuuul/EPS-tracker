using System.Collections.Generic;
using System.Globalization;
using app.DTO;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class BrojiloMerenjaDialog : Dialog
    {
        public BrojiloMerenjaDialog(Window parent, string serijskiBroj, IList<MerenjeListDto> merenja)
            : base("Merenja brojila", parent, DialogFlags.Modal)
        {
            SetDefaultSize(900, 420);
            DestroyWithParent = true;

            var title = new Label("Brojilo: " + serijskiBroj)
            {
                Xalign = 0,
                MarginBottom = 8
            };

            ContentArea.BorderWidth = 12;
            ContentArea.PackStart(title, false, false, 0);

            if (merenja == null || merenja.Count == 0)
            {
                ContentArea.PackStart(new Label("Brojilo nema evidentirana merenja.")
                {
                    Xalign = 0
                }, false, false, 0);
            }
            else
            {
                ListStore store;
                var table = ViewFactory.Table(
                    out store,
                    "ID",
                    "Datum i vreme",
                    "Aktivna kWh",
                    "Reaktivna kWh",
                    "Snaga",
                    "Napon",
                    "Struja",
                    "Tip",
                    "Izvor",
                    "Validirano");

                foreach (var merenje in merenja)
                {
                    store.AppendValues(
                        merenje.Id.ToString(CultureInfo.InvariantCulture),
                        merenje.DatumVremeMerenja.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        FormatDecimal(merenje.PotrosnjaAktivna),
                        FormatDecimal(merenje.PotrosnjaReaktivna),
                        FormatDecimal(merenje.Snaga),
                        FormatDecimal(merenje.Napon),
                        FormatDecimal(merenje.Struja),
                        merenje.TipMerenja ?? string.Empty,
                        merenje.TipIzvora ?? string.Empty,
                        merenje.IsValidirano ?? string.Empty);
                }

                ContentArea.PackStart(ViewFactory.Scroll(table), true, true, 0);
            }

            AddButton("Zatvori", ResponseType.Close);
        }

        private static string FormatDecimal(decimal? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }
    }
}
