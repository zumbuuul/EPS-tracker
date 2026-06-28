using System.Collections.Generic;
using System.Globalization;
using app.DTO;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class PotrosacBrojilaDialog : Dialog
    {
        public PotrosacBrojilaDialog(Window parent, long potrosacId, IList<BrojiloListDto> brojila)
            : base("Brojila potrosaca", parent, DialogFlags.Modal)
        {
            SetDefaultSize(760, 420);
            DestroyWithParent = true;

            var title = new Label("Potrosac ID: " + potrosacId.ToString(CultureInfo.InvariantCulture))
            {
                Xalign = 0,
                MarginBottom = 8
            };

            ContentArea.BorderWidth = 12;
            ContentArea.PackStart(title, false, false, 0);

            if (brojila == null || brojila.Count == 0)
            {
                ContentArea.PackStart(new Label("Potrosac nema povezana brojila.")
                {
                    Xalign = 0
                }, false, false, 0);
            }
            else
            {
                ListStore store;
                var table = ViewFactory.Table(
                    out store,
                    "Serijski broj",
                    "Tipovi",
                    "Status",
                    "Lokacija",
                    "Datum instalacije",
                    "Poslednja zamena",
                    "Koeficijent");

                foreach (var brojilo in brojila)
                {
                    store.AppendValues(
                        brojilo.SerijskiBroj ?? string.Empty,
                        brojilo.TipoviBrojila == null ? string.Empty : string.Join(", ", brojilo.TipoviBrojila),
                        brojilo.Status ?? string.Empty,
                        brojilo.Lokacija ?? string.Empty,
                        brojilo.DatumInstalacije.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        brojilo.PoslednjiDatumZamene.HasValue
                            ? brojilo.PoslednjiDatumZamene.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                            : string.Empty,
                        brojilo.KoeficijentMnozenja.HasValue
                            ? brojilo.KoeficijentMnozenja.Value.ToString(CultureInfo.InvariantCulture)
                            : string.Empty);
                }

                ContentArea.PackStart(ViewFactory.Scroll(table), true, true, 0);
            }

            AddButton("Zatvori", ResponseType.Close);
        }
    }
}
