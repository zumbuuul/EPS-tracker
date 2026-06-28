using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class RacunDialog : FormDialog
    {
        public RacunDialog(Window parent)
            : base("Racun", parent)
        {
            AddSection("Veze");
            AddEntry("Broj racuna");
            AddEntry("Potrosac ID");
            AddEntry("Serijski broj brojila");
            AddEntry("Merenje ID");

            AddSection("Period i potrosnja");
            AddEntry("Period od", "yyyy-mm-dd");
            AddEntry("Period do", "yyyy-mm-dd");
            AddDecimal("Ukupna potrosnja");

            AddSection("Iznosi");
            AddDecimal("Iznos bez PDV");
            AddDecimal("PDV");
            AddDecimal("Ukupan iznos");

            AddSection("Placanje");
            AddEntry("Datum izdavanja", "yyyy-mm-dd");
            AddEntry("Rok placanja", "yyyy-mm-dd");
            AddEntry("Status");
            AddEntry("Nacin placanja");
            AddText("Komentar");
        }
    }
}
