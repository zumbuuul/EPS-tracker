using System;
using app.Entities.Enums;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class MerenjeDialog : FormDialog
    {
        public MerenjeDialog(Window parent)
            : base("Merenje", parent)
        {
            AddSection("Veza");
            AddEntry("Serijski broj brojila");
            AddEntry("Datum i vreme", "yyyy-mm-dd hh:mm");

            AddSection("Elektricna merenja");
            AddDecimal("Aktivna potrosnja kWh");
            AddDecimal("Reaktivna potrosnja kWh");
            AddDecimal("Snaga");
            AddDecimal("Napon");
            AddDecimal("Struja");

            AddSection("Klasifikacija");
            AddEntry("Tip merenja");
            AddCombo("Izvor podataka", Enum.GetNames(typeof(TipIzvoraMerenja)));
            AddCombo("Validirano", Enum.GetNames(typeof(DaNe)));
            AddText("Komentar");
        }
    }
}
