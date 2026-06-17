using System;
using app.Views.Dialogs;
using app.Views.Ui;
using Gtk;

namespace app.Views.Pages
{
    public class RacuniPage : ListPage
    {
        public RacuniPage(Action<string> showStatus)
            : base("Racuni", "Racuni generisani iz merenja za potrosaca i brojilo.", showStatus)
        {
            AddFilter(ViewFactory.Search("Pretraga racuna"));
            AddFilter(ViewFactory.Combo("Svi statusi", "IZDAT", "PLACEN", "KASNJENJE", "STORNIRAN"));
            AddFilter(ViewFactory.Combo("Nacin placanja", "Svi", "UPLATNICA", "ONLINE", "TRAJNI_NALOG"));

            AddAction("Dodaj", "list-add", "Novi racun", (sender, args) =>
                OpenDialog(new RacunDialog(DialogParent), "Racun je spreman za cuvanje kroz service sloj."));
            AddAction("Izmeni", "document-edit", "Izmena odabranog racuna", (sender, args) =>
                Report("Izmena racuna ce koristiti odabrani red i RacunService."));
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog racuna", (sender, args) =>
                Report("Brisanje racuna ce ici kroz RacunService."));
            AddAction("Placeno", "emblem-ok", "Oznaci racun kao placen", (sender, args) =>
                Report("Promena statusa racuna ce ici kroz RacunService."));

            SetTable(
                "Broj racuna",
                "Potrosac",
                "Brojilo",
                "Period",
                "Ukupna potrosnja",
                "Iznos bez PDV",
                "PDV",
                "Ukupno",
                "Status",
                "Rok placanja");
        }
    }
}
