using System;
using app.DTO;
using app.Entities.Enums;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class PotrosacDialog : FormDialog
    {
        private static readonly string[] TipNames = Enum.GetNames(typeof(PotrosacTip));

        private readonly long id;
        private readonly ComboBoxText tipCombo;
        private readonly Entry statusEntry;
        private readonly Entry kategorijaTarifeEntry;
        private readonly Entry imeEntry;
        private readonly Entry prezimeEntry;
        private readonly Entry jmbgEntry;
        private readonly Entry nazivEntry;
        private readonly Entry pibEntry;
        private readonly Entry adresaEntry;
        private readonly Entry gradEntry;
        private readonly Entry telefonEntry;
        private readonly Entry emailEntry;
        private readonly TextView komentarText;

        public PotrosacDialog(Window parent)
            : this(parent, null)
        {
        }

        public PotrosacDialog(Window parent, PotrosacDto potrosac)
            : base("Potrosac", parent)
        {
            id = potrosac != null ? potrosac.Id : 0;

            AddSection("Osnovni podaci");
            tipCombo = AddCombo("Tip potrosaca", TipNames);
            statusEntry = AddEntry("Status");
            kategorijaTarifeEntry = AddEntry("Kategorija tarife");

            AddSection("Fizicko lice");
            imeEntry = AddEntry("Ime");
            prezimeEntry = AddEntry("Prezime");
            jmbgEntry = AddEntry("JMBG");

            AddSection("Pravno lice");
            nazivEntry = AddEntry("Naziv");
            pibEntry = AddEntry("PIB");

            AddSection("Kontakt");
            adresaEntry = AddEntry("Adresa");
            gradEntry = AddEntry("Grad");
            telefonEntry = AddEntry("Telefon");
            emailEntry = AddEntry("Email");
            komentarText = AddText("Komentar");

            if (potrosac != null)
            {
                Fill(potrosac);
            }
        }

        public PotrosacSaveDto ToSaveDto()
        {
            return new PotrosacSaveDto
            {
                Id = id,
                Tip = (PotrosacTip)Enum.Parse(typeof(PotrosacTip), tipCombo.ActiveText),
                Email = emailEntry.Text,
                Telefon = telefonEntry.Text,
                Adresa = adresaEntry.Text,
                Grad = gradEntry.Text,
                Komentar = komentarText.Buffer.Text,
                Status = statusEntry.Text,
                KategorijaTarife = kategorijaTarifeEntry.Text,
                Domacinstvo = new DomacinstvoDto
                {
                    Id = id,
                    Ime = imeEntry.Text,
                    Prezime = prezimeEntry.Text,
                    Jmbg = jmbgEntry.Text
                },
                Firma = new FirmaDto
                {
                    Id = id,
                    Naziv = nazivEntry.Text,
                    Pib = pibEntry.Text
                }
            };
        }

        private void Fill(PotrosacDto potrosac)
        {
            SetActiveText(tipCombo, potrosac.Tip.ToString());
            statusEntry.Text = potrosac.Status ?? string.Empty;
            kategorijaTarifeEntry.Text = potrosac.KategorijaTarife ?? string.Empty;
            adresaEntry.Text = potrosac.Adresa ?? string.Empty;
            gradEntry.Text = potrosac.Grad ?? string.Empty;
            telefonEntry.Text = potrosac.Telefon ?? string.Empty;
            emailEntry.Text = potrosac.Email ?? string.Empty;
            komentarText.Buffer.Text = potrosac.Komentar ?? string.Empty;

            if (potrosac.Domacinstvo != null)
            {
                imeEntry.Text = potrosac.Domacinstvo.Ime ?? string.Empty;
                prezimeEntry.Text = potrosac.Domacinstvo.Prezime ?? string.Empty;
                jmbgEntry.Text = potrosac.Domacinstvo.Jmbg ?? string.Empty;
            }

            if (potrosac.Firma != null)
            {
                nazivEntry.Text = potrosac.Firma.Naziv ?? string.Empty;
                pibEntry.Text = potrosac.Firma.Pib ?? string.Empty;
            }
        }

        private static void SetActiveText(ComboBoxText combo, string value)
        {
            for (var index = 0; index < TipNames.Length; index++)
            {
                if (TipNames[index] == value)
                {
                    combo.Active = index;
                    return;
                }
            }
        }
    }
}
