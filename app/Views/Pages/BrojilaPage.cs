using System;
using app.Views.Dialogs;
using app.Views.Ui;
using Gtk;

namespace app.Views.Pages
{
    public class BrojilaPage : ListPage
    {
        public BrojilaPage(Action<string> showStatus)
            : base("Brojila", "Serijski brojevi, tipovi brojila, zamene i tehnicke osobine.", showStatus)
        {
            AddFilter(ViewFactory.Search("Pretraga brojila"));
            AddFilter(ViewFactory.Combo("Svi tipovi", "JEDNOFAZNO", "TROFAZNO", "PAMETNO", "MEHANICKO", "RASVETNO"));
            AddFilter(ViewFactory.Combo("Svi statusi", "AKTIVNO", "ZAMENJENO", "NEAKTIVNO"));

            AddAction("Dodaj", "list-add", "Novo brojilo", (sender, args) =>
                OpenDialog(new BrojiloDialog(DialogParent), "Brojilo je spremno za cuvanje kroz service sloj."));
            AddAction("Izmeni", "document-edit", "Izmena odabranog brojila", (sender, args) =>
                Report("Izmena brojila ce koristiti odabrani red i BrojiloService."));
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog brojila", (sender, args) =>
                Report("Brisanje brojila ce ici kroz BrojiloService."));
            AddAction("Merenja", "accessories-calculator", "Merenja za odabrano brojilo", (sender, args) =>
                Report("Merenja odabranog brojila ce se otvoriti kroz MerenjeService."));
            AddAction("Kvar", "dialog-warning", "Prijavi kvar za brojilo", (sender, args) =>
                OpenDialog(new KvarDialog(DialogParent), "Kvar je spreman za cuvanje kroz service sloj."));

            SetTable(
                "Serijski broj",
                "Tipovi",
                "Status",
                "Lokacija",
                "Potrosaci",
                "Datum instalacije",
                "Datumi zamene",
                "Koeficijent");
        }
    }
}
