using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class StanjeDialog : FormDialog
    {
        public StanjeDialog(Window parent)
            : base("Stanje mreze", parent)
        {
            AddSection("Lokacija");
            AddEntry("Datum i vreme", "yyyy-mm-dd hh:mm");
            AddEntry("Lokacija stanice");
            AddEntry("Transformator");
            AddDecimal("Naponski nivo");
            AddEntry("Status mreze");

            AddSection("Energija");
            AddDecimal("Proizvedena energija");
            AddDecimal("Distribuirana energija");
            AddDecimal("Gubici u mrezi");
            AddDecimal("Ukupna potrosnja");
            AddText("Komentar");
        }
    }
}
