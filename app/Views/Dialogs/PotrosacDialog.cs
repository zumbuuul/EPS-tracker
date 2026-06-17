using System;
using app.Entities.Enums;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class PotrosacDialog : FormDialog
    {
        public PotrosacDialog(Window parent)
            : base("Potrosac", parent)
        {
            AddSection("Osnovni podaci");
            AddCombo("Tip potrosaca", Enum.GetNames(typeof(PotrosacTip)));
            AddEntry("Status");
            AddEntry("Kategorija tarife");

            AddSection("Fizicko lice");
            AddEntry("Ime");
            AddEntry("Prezime");
            AddEntry("JMBG");

            AddSection("Pravno lice");
            AddEntry("Naziv");
            AddEntry("PIB");

            AddSection("Kontakt");
            AddEntry("Adresa");
            AddEntry("Grad");
            AddEntry("Telefon");
            AddEntry("Email");
            AddText("Komentar");
        }
    }
}
