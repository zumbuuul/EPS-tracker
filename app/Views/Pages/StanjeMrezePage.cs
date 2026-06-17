using System;
using app.Views.Dialogs;
using app.Views.Ui;
using Gtk;

namespace app.Views.Pages
{
    public class StanjeMrezePage : ListPage
    {
        public StanjeMrezePage(Action<string> showStatus)
            : base("Stanje mreze", "Proizvodnja, distribucija, gubici i potrosnja po stanici.", showStatus)
        {
            AddFilter(ViewFactory.Search("Pretraga stanice"));
            AddFilter(ViewFactory.Combo("Svi statusi", "STABILNO", "OPTERECENO", "PREKID", "ODRZAVANJE"));

            AddAction("Dodaj", "list-add", "Novo stanje mreze", (sender, args) =>
                OpenDialog(new StanjeDialog(DialogParent), "Stanje mreze je spremno za cuvanje kroz service sloj."));
            AddAction("Izmeni", "document-edit", "Izmena odabranog stanja", (sender, args) =>
                Report("Izmena stanja mreze ce koristiti odabrani red i StanjeService."));
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog stanja", (sender, args) =>
                Report("Brisanje stanja mreze ce ici kroz StanjeService."));
            AddAction("Osvezi", "view-refresh", "Osvezi stanje mreze", (sender, args) =>
                Report("Osvezavanje stanja mreze ce ici kroz StanjeService."));

            SetTable(
                "Datum i vreme",
                "Lokacija",
                "Transformator",
                "Naponski nivo",
                "Proizvedena",
                "Distribuirana",
                "Gubici",
                "Ukupna potrosnja",
                "Status");
        }
    }
}
