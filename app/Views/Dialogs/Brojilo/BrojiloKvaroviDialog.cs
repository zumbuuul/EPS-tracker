using System.Collections.Generic;
using System.Globalization;
using app.DTO;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class BrojiloKvaroviDialog : Dialog
    {
        public BrojiloKvaroviDialog(Window parent, string serijskiBroj, IList<KvarListDto> kvarovi)
            : base("Kvarovi brojila", parent, DialogFlags.Modal)
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

            if (kvarovi == null || kvarovi.Count == 0)
            {
                ContentArea.PackStart(new Label("Brojilo nema evidentirane kvarove.")
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
                    "Potrosac",
                    "Datum prijave",
                    "Tip kvara",
                    "Status",
                    "Prioritet",
                    "Tim",
                    "Trajanje");

                foreach (var kvar in kvarovi)
                {
                    store.AppendValues(
                        kvar.Id.ToString(CultureInfo.InvariantCulture),
                        kvar.ImeIliNazivPotrosaca ?? kvar.PotrosacId.ToString(CultureInfo.InvariantCulture),
                        kvar.DatumPrijave.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        kvar.TipKvara ?? string.Empty,
                        kvar.Status ?? string.Empty,
                        kvar.Prioritet ?? string.Empty,
                        kvar.NadlezniTim ?? string.Empty,
                        kvar.TrajanjeUSatima.HasValue
                            ? kvar.TrajanjeUSatima.Value.ToString(CultureInfo.InvariantCulture)
                            : string.Empty);
                }

                ContentArea.PackStart(ViewFactory.Scroll(table), true, true, 0);
            }

            AddButton("Zatvori", ResponseType.Close);
        }
    }
}
