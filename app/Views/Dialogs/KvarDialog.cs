using System;
using app.Entities.Enums;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class KvarDialog : FormDialog
    {
        public KvarDialog(Window parent)
            : base("Kvar", parent)
        {
            AddSection("Veze");
            AddEntry("Serijski broj brojila");
            AddEntry("Potrosac ID");

            AddSection("Prijava");
            AddEntry("Datum prijave", "yyyy-mm-dd hh:mm");
            AddEntry("Tip kvara");
            AddCombo("Status", Enum.GetNames(typeof(KvarStatus)));
            AddCombo("Prioritet", Enum.GetNames(typeof(KvarPrioritet)));
            AddEntry("Nadlezni tim");

            AddSection("Otklanjanje");
            AddEntry("Datum otklanjanja", "yyyy-mm-dd hh:mm");
            AddDecimal("Trajanje u satima");
            AddText("Opis problema");
            AddText("Komentar");
        }
    }
}
