using System;
using app.Views.Dialogs;
using app.Views.Ui;
using Gtk;

namespace app.Views.Pages
{
    public class PotrosaciPage : ListPage
    {
        public PotrosaciPage(Action<string> showStatus)
            : base("Potrosaci", "Domacinstva, firme, institucije i javna rasveta.", showStatus)
        {
            AddFilter(ViewFactory.Search("Pretraga potrosaca"));
            AddFilter(ViewFactory.Combo("Svi tipovi", "DOMACINSTVO", "FIRMA", "INSTITUCIJA", "JAVNA_RASVETA"));

            AddAction("Dodaj", "list-add", "Novi potrosac", (sender, args) =>
                OpenDialog(new PotrosacDialog(DialogParent), "Potrosac je spreman za cuvanje kroz service sloj."));
            AddAction("Izmeni", "document-edit", "Izmena odabranog potrosaca", (sender, args) =>
                Report("Izmena potrosaca ce koristiti odabrani red i PotrosacService."));
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog potrosaca", (sender, args) =>
                Report("Brisanje potrosaca ce ici kroz PotrosacService."));
            AddAction("Povezi", "insert-link", "Povezi potrosaca i brojilo", (sender, args) =>
                Report("Povezivanje potrosaca i brojila pripada PotrosacService/BrojiloService granici."));
            AddAction("Brojila", "view-list-symbolic", "Prikazi brojila potrosaca", (sender, args) =>
                Report("Lista brojila za potrosaca ce se ucitati iz odabranog reda."));

            SetTable(
                "ID",
                "Tip",
                "Ime/Naziv",
                "Grad",
                "Telefon",
                "Email",
                "Status",
                "Tarifa",
                "Brojila");
        }
    }
}
