using System;
using System.Linq;
using System.Net.Mail;
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
        private readonly Label fizickoLiceSection;
        private readonly Entry imeEntry;
        private readonly Entry prezimeEntry;
        private readonly Entry jmbgEntry;
        private readonly Label pravnoLiceSection;
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

            fizickoLiceSection = AddSection("Fizicko lice");
            imeEntry = AddEntry("Ime");
            prezimeEntry = AddEntry("Prezime");
            jmbgEntry = AddEntry("JMBG");
            AttachDigitsOnly(jmbgEntry, 13);

            pravnoLiceSection = AddSection("Pravno lice");
            nazivEntry = AddEntry("Naziv");
            pibEntry = AddEntry("PIB");
            AttachDigitsOnly(pibEntry, 9);

            AddSection("Kontakt");
            adresaEntry = AddEntry("Adresa");
            gradEntry = AddEntry("Grad");
            telefonEntry = AddEntry("Telefon");
            AttachDigitsOnly(telefonEntry, 0);
            emailEntry = AddEntry("Email");
            komentarText = AddText("Komentar");

            tipCombo.Changed += (sender, args) => UpdateConditionalSections();

            if (potrosac != null)
            {
                Fill(potrosac);
            }

            UpdateConditionalSections();
        }

        public PotrosacSaveDto ToSaveDto()
        {
            Validate();
            var tip = ActiveTip();

            return new PotrosacSaveDto
            {
                Id = id,
                Tip = tip.ToString(),
                Email = emailEntry.Text,
                Telefon = telefonEntry.Text,
                Adresa = adresaEntry.Text,
                Grad = gradEntry.Text,
                Komentar = komentarText.Buffer.Text,
                Status = statusEntry.Text,
                KategorijaTarife = kategorijaTarifeEntry.Text,
                Domacinstvo = tip == PotrosacTip.DOMACINSTVO
                    ? BuildDomacinstvoDto()
                    : null,
                Firma = tip == PotrosacTip.FIRMA
                    ? BuildFirmaDto()
                    : null
            };
        }

        private void UpdateConditionalSections()
        {
            var tip = ActiveTip();
            var showFizickoLice = tip == PotrosacTip.DOMACINSTVO;
            var showPravnoLice = tip == PotrosacTip.FIRMA;

            SetWidgetVisible(fizickoLiceSection, showFizickoLice);
            SetRowVisible(imeEntry, showFizickoLice);
            SetRowVisible(prezimeEntry, showFizickoLice);
            SetRowVisible(jmbgEntry, showFizickoLice);

            SetWidgetVisible(pravnoLiceSection, showPravnoLice);
            SetRowVisible(nazivEntry, showPravnoLice);
            SetRowVisible(pibEntry, showPravnoLice);
        }

        private PotrosacTip ActiveTip()
        {
            return (PotrosacTip)Enum.Parse(typeof(PotrosacTip), tipCombo.ActiveText);
        }

        private DomacinstvoDto BuildDomacinstvoDto()
        {
            return new DomacinstvoDto
            {
                Id = id,
                Ime = imeEntry.Text,
                Prezime = prezimeEntry.Text,
                Jmbg = jmbgEntry.Text
            };
        }

        private FirmaDto BuildFirmaDto()
        {
            return new FirmaDto
            {
                Id = id,
                Naziv = nazivEntry.Text,
                Pib = pibEntry.Text
            };
        }

        private void Fill(PotrosacDto potrosac)
        {
            SetActiveText(tipCombo, potrosac.Tip);
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

        private void Validate()
        {
            var tip = ActiveTip();
            var email = emailEntry.Text.Trim();
            var telefon = telefonEntry.Text.Trim();

            if (telefon.Length > 0 && !IsDigitsOnly(telefon))
            {
                throw new ArgumentException("Telefon sme da sadrzi samo cifre.");
            }

            if (email.Length > 0 && !IsValidEmail(email))
            {
                throw new ArgumentException("Email mora biti u ispravnom formatu.");
            }

            if (tip == PotrosacTip.DOMACINSTVO && !HasExactDigitCount(jmbgEntry.Text, 13))
            {
                throw new ArgumentException("JMBG mora imati tacno 13 cifara.");
            }

            if (tip == PotrosacTip.FIRMA && !HasExactDigitCount(pibEntry.Text, 9))
            {
                throw new ArgumentException("PIB mora imati tacno 9 cifara.");
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

        private static void AttachDigitsOnly(Entry entry, int maxLength)
        {
            var updating = false;

            if (maxLength > 0)
            {
                entry.MaxLength = maxLength;
            }

            entry.Changed += (sender, args) =>
            {
                //provera da li je promena rezultat linije 196
                if (updating)
                {
                    return;
                }

                var text = entry.Text ?? string.Empty;
                var digits = new string(text.Where(IsAsciiDigit).ToArray());

                if (maxLength > 0 && digits.Length > maxLength)
                {
                    digits = digits.Substring(0, maxLength);
                }

                if (digits == text)
                {
                    return;
                }

                updating = true;
                entry.Text = digits;
                entry.Position = digits.Length;
                updating = false;
            };
        }

        private static bool HasExactDigitCount(string value, int count)
        {
            var text = value == null ? string.Empty : value.Trim();
            return text.Length == count && IsDigitsOnly(text);
        }

        private static bool IsDigitsOnly(string value)
        {
            return value != null && value.Length > 0 && value.All(IsAsciiDigit);
        }

        private static bool IsAsciiDigit(char value)
        {
            return value >= '0' && value <= '9';
        }

        private static bool IsValidEmail(string value)
        {
            try
            {
                var address = new MailAddress(value);
                return address.Address == value;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
