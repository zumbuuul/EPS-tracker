using System;
using app.Views.Dialogs;
using app.Views.Ui;
using Gtk;

namespace app.Views.Pages
{
    public class MerenjaPage : ListPage
    {
        public MerenjaPage(Action<string> showStatus)
            : base("Merenja", "Stvarna potrosnja, izvor ocitavanja i validacija.", showStatus)
        {
            AddFilter(ViewFactory.Search("Pretraga merenja"));
            AddFilter(ViewFactory.Combo("Svi izvori", "RUCNO_OCITAVANJE", "PAMETNO_BROJILO"));
            AddFilter(ViewFactory.Combo("Validacija", "Sva", "D", "N"));

            AddAction("Dodaj", "list-add", "Novo merenje", (sender, args) =>
                OpenDialog(new MerenjeDialog(DialogParent), "Merenje je spremno za cuvanje kroz service sloj."));
            AddAction("Izmeni", "document-edit", "Izmena odabranog merenja", (sender, args) =>
                Report("Izmena merenja ce koristiti odabrani red i MerenjeService."));
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog merenja", (sender, args) =>
                Report("Brisanje merenja ce ici kroz MerenjeService."));
            AddAction("Validiraj", "emblem-ok", "Validiraj odabrano merenje", (sender, args) =>
                Report("Validacija merenja ce se obraditi bez message box-a, kroz status i service rezultat."));
            AddAction("Racun", "x-office-spreadsheet", "Generisi racun iz merenja", (sender, args) =>
                OpenDialog(new RacunDialog(DialogParent), "Racun je spreman za generisanje kroz RacunService."));

            SetTable(
                "ID",
                "Brojilo",
                "Datum i vreme",
                "Aktivna kWh",
                "Reaktivna kWh",
                "Snaga",
                "Napon",
                "Struja",
                "Izvor",
                "Validirano");
        }
    }
}
