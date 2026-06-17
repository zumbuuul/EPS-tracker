using System;
using app.Views.Dialogs;
using app.Views.Ui;
using Gtk;

namespace app.Views.Pages
{
    public class KvaroviPage : ListPage
    {
        public KvaroviPage(Action<string> showStatus)
            : base("Kvarovi", "Prijave, prioriteti, timovi i otklanjanje kvarova.", showStatus)
        {
            AddFilter(ViewFactory.Search("Pretraga kvarova"));
            AddFilter(ViewFactory.Combo("Svi statusi", "PRIJAVLJEN", "U_OBRADI", "OTKLONJEN", "ZATVOREN"));
            AddFilter(ViewFactory.Combo("Svi prioriteti", "NIZAK", "SREDNJI", "VISOK", "KRITICAN"));

            AddAction("Dodaj", "list-add", "Nova prijava kvara", (sender, args) =>
                OpenDialog(new KvarDialog(DialogParent), "Kvar je spreman za cuvanje kroz service sloj."));
            AddAction("Izmeni", "document-edit", "Izmena odabranog kvara", (sender, args) =>
                Report("Izmena kvara ce koristiti odabrani red i KvarService."));
            AddAction("Obrisi", "edit-delete", "Brisanje odabranog kvara", (sender, args) =>
                Report("Brisanje kvara ce ici kroz KvarService."));
            AddAction("Otkloni", "emblem-ok", "Oznaci kvar kao otklonjen", (sender, args) =>
                Report("Otklanjanje kvara ce azurirati status bez message box-a."));

            SetTable(
                "ID",
                "Tip kvara",
                "Brojilo",
                "Potrosac",
                "Datum prijave",
                "Status",
                "Prioritet",
                "Nadlezni tim",
                "Trajanje",
                "Otklonjeno");
        }
    }
}
