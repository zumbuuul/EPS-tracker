using System;
using System.Globalization;
using app.DTO;
using app.Entities.Enums;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class MerenjeDialog : FormDialog
    {
        private static readonly string[] TipIzvoraNames = Enum.GetNames(typeof(TipIzvoraMerenja));

        private readonly long id;
        private readonly Entry serijskiBrojEntry;
        private readonly Entry datumVremeEntry;
        private readonly SpinButton aktivnaPotrosnjaSpin;
        private readonly SpinButton reaktivnaPotrosnjaSpin;
        private readonly SpinButton snagaSpin;
        private readonly SpinButton naponSpin;
        private readonly SpinButton strujaSpin;
        private readonly Entry tipMerenjaEntry;
        private readonly ComboBoxText tipIzvoraCombo;
        private readonly CheckButton validiranoCheck;
        private readonly TextView komentarText;

        public MerenjeDialog(Window parent)
            : this(parent, null)
        {
        }

        public MerenjeDialog(Window parent, MerenjeDto merenje)
            : base("Merenje", parent)
        {
            id = merenje != null ? merenje.Id : 0;

            AddSection("Veza");
            serijskiBrojEntry = AddEntry("Serijski broj brojila");
            datumVremeEntry = AddEntry("Datum i vreme", "yyyy-MM-dd HH:mm");

            AddSection("Elektricna merenja");
            aktivnaPotrosnjaSpin = AddDecimal("Aktivna potrosnja kWh");
            reaktivnaPotrosnjaSpin = AddDecimal("Reaktivna potrosnja kWh");
            snagaSpin = AddDecimal("Snaga");
            naponSpin = AddDecimal("Napon");
            strujaSpin = AddDecimal("Struja");

            AddSection("Klasifikacija");
            tipMerenjaEntry = AddEntry("Tip merenja");
            tipIzvoraCombo = AddCombo("Izvor podataka", TipIzvoraNames);
            validiranoCheck = AddCheck("Validirano");
            komentarText = AddText("Komentar");

            if (merenje != null)
            {
                Fill(merenje);
            }
        }

        public MerenjeSaveDto ToSaveDto()
        {
            Validate();

            return new MerenjeSaveDto
            {
                Id = id,
                SerijskiBroj = serijskiBrojEntry.Text,
                DatumVremeMerenja = ParseDateTime(datumVremeEntry.Text),
                PotrosnjaAktivna = DecimalValue(aktivnaPotrosnjaSpin),
                PotrosnjaReaktivna = DecimalValue(reaktivnaPotrosnjaSpin),
                Snaga = DecimalValue(snagaSpin),
                Napon = DecimalValue(naponSpin),
                Struja = DecimalValue(strujaSpin),
                TipMerenja = tipMerenjaEntry.Text,
                TipIzvora = tipIzvoraCombo.ActiveText,
                IsValidirano = validiranoCheck.Active ? "DA" : "NE",
                Komentar = komentarText.Buffer.Text
            };
        }

        private void Fill(MerenjeDto merenje)
        {
            serijskiBrojEntry.Text = merenje.SerijskiBroj ?? string.Empty;
            datumVremeEntry.Text = merenje.DatumVremeMerenja.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            SetDecimal(aktivnaPotrosnjaSpin, merenje.PotrosnjaAktivna);
            SetDecimal(reaktivnaPotrosnjaSpin, merenje.PotrosnjaReaktivna);
            SetDecimal(snagaSpin, merenje.Snaga);
            SetDecimal(naponSpin, merenje.Napon);
            SetDecimal(strujaSpin, merenje.Struja);
            tipMerenjaEntry.Text = merenje.TipMerenja ?? string.Empty;
            SetActiveText(tipIzvoraCombo, merenje.TipIzvora);
            validiranoCheck.Active = string.Equals(merenje.IsValidirano, "DA", StringComparison.OrdinalIgnoreCase)
                || string.Equals(merenje.IsValidirano, "D", StringComparison.OrdinalIgnoreCase);
            komentarText.Buffer.Text = merenje.Komentar ?? string.Empty;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(serijskiBrojEntry.Text))
            {
                throw new ArgumentException("Serijski broj brojila je obavezan.");
            }

            ParseDateTime(datumVremeEntry.Text);

            if (string.IsNullOrWhiteSpace(tipMerenjaEntry.Text))
            {
                throw new ArgumentException("Tip merenja je obavezan.");
            }

            if (string.IsNullOrWhiteSpace(tipIzvoraCombo.ActiveText))
            {
                throw new ArgumentException("Izvor podataka je obavezan.");
            }

            if (string.IsNullOrWhiteSpace(komentarText.Buffer.Text))
            {
                throw new ArgumentException("Komentar je obavezan.");
            }
        }

        private static DateTime ParseDateTime(string value)
        {
            DateTime dateTime;

            if (!DateTime.TryParseExact(
                value == null ? string.Empty : value.Trim(),
                "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime))
            {
                throw new ArgumentException("Datum i vreme moraju biti u formatu yyyy-MM-dd HH:mm.");
            }

            return dateTime;
        }

        private static void SetActiveText(ComboBoxText combo, string value)
        {
            for (var index = 0; index < TipIzvoraNames.Length; index++)
            {
                if (TipIzvoraNames[index] == value)
                {
                    combo.Active = index;
                    return;
                }
            }
        }

        private static decimal DecimalValue(SpinButton spin)
        {
            return Convert.ToDecimal(spin.Value);
        }

        private static void SetDecimal(SpinButton spin, decimal? value)
        {
            spin.Value = value.HasValue ? Convert.ToDouble(value.Value) : 0;
        }
    }
}
