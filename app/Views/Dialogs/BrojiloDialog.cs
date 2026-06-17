using System;
using app.Entities.Enums;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class BrojiloDialog : FormDialog
    {
        public BrojiloDialog(Window parent)
            : base("Brojilo", parent)
        {
            AddSection("Osnovni podaci");
            AddEntry("Serijski broj");
            AddEntry("Datum instalacije", "yyyy-mm-dd");
            AddEntry("Datumi zamene", "yyyy-mm-dd, yyyy-mm-dd");
            AddEntry("Status");
            AddEntry("Lokacija");
            AddDecimal("Koeficijent mnozenja");
            AddText("Komentar");

            AddSection("Tipovi brojila");
            foreach (var tip in Enum.GetNames(typeof(TipBrojila)))
            {
                AddCheck(tip);
            }

            AddSection("Mehanicko brojilo");
            AddEntry("Poslednja kalibracija", "yyyy-mm-dd");
            AddEntry("Preciznost klase");
            AddDecimal("Maksimalna greska");

            AddSection("Pametno brojilo");
            AddCombo("Protokol", new[] { "GSM", "GPRS" });
            AddDecimal("Frekvencija slanja");
            AddCombo("Daljinsko iskljucenje", Enum.GetNames(typeof(DaNe)));
            AddDecimal("Nivo baterije");

            AddSection("Trofazno brojilo");
            AddDecimal("Maksimalna snaga");
            AddCombo("Merenje po zonama", Enum.GetNames(typeof(DaNe)));
            AddDecimal("Ugovorena snaga");

            AddSection("Javna rasveta");
            AddEntry("Vreme ukljucenja");
            AddEntry("Vreme iskljucenja");
            AddEntry("Status fotosenzora");
            AddDecimal("Nocno vreme rada");
        }
    }
}
