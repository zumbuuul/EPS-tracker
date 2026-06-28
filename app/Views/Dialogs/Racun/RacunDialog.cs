using System;
using System.Globalization;
using app.DTO;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class RacunDialog : FormDialog
    {
        private static readonly string[] Statusi =
        {
            "POSLAT",
            "PLACEN",
            "KASNJENJE",
            "STORNIRAN"
        };

        private static readonly string[] NaciniPlacanja =
        {
            "UPLATNICA",
            "VIRMAN",
            "ONLINE",
            "TRAJNI_NALOG"
        };

        private readonly Entry brojRacunaEntry;
        private readonly Entry potrosacIdEntry;
        private readonly Entry merenjeIdEntry;
        private readonly Entry periodOdEntry;
        private readonly Entry periodDoEntry;
        private readonly SpinButton iznosBezPdvSpin;
        private readonly SpinButton pdvSpin;
        private readonly Entry datumIzdavanjaEntry;
        private readonly Entry rokPlacanjaEntry;
        private readonly ComboBoxText statusCombo;
        private readonly ComboBoxText nacinPlacanjaCombo;
        private readonly TextView komentarText;

        public RacunDialog(Window parent)
            : this(parent, null)
        {
        }

        public RacunDialog(Window parent, RacunDto racun)
            : base("Racun", parent)
        {
            AddSection("Veze");
            brojRacunaEntry = AddEntry("Broj racuna");
            potrosacIdEntry = AddEntry("Potrosac ID");
            merenjeIdEntry = AddEntry("Merenje ID");

            AddSection("Period i potrosnja");
            periodOdEntry = AddEntry("Period od", "yyyy-MM-dd");
            periodDoEntry = AddEntry("Period do", "yyyy-MM-dd");

            AddSection("Iznosi");
            iznosBezPdvSpin = AddDecimal("Iznos bez PDV");
            pdvSpin = AddDecimal("PDV");

            AddSection("Placanje");
            datumIzdavanjaEntry = AddEntry("Datum izdavanja", "yyyy-MM-dd");
            rokPlacanjaEntry = AddEntry("Rok placanja", "yyyy-MM-dd");
            statusCombo = AddCombo("Status", Statusi);
            nacinPlacanjaCombo = AddCombo("Nacin placanja", NaciniPlacanja);
            komentarText = AddText("Komentar");

            if (racun != null)
            {
                Fill(racun);
                brojRacunaEntry.Sensitive = false;
            }
        }

        public RacunSaveDto ToSaveDto()
        {
            Validate();

            return new RacunSaveDto
            {
                BrojRacuna = brojRacunaEntry.Text,
                PotrosacId = ParseLong(potrosacIdEntry.Text, "Potrosac ID mora biti broj."),
                MerenjeId = ParseLong(merenjeIdEntry.Text, "Merenje ID mora biti broj."),
                DatumIzdavanja = ParseDate(datumIzdavanjaEntry.Text, "Datum izdavanja mora biti u formatu yyyy-MM-dd."),
                RokPlacanja = ParseDate(rokPlacanjaEntry.Text, "Rok placanja mora biti u formatu yyyy-MM-dd."),
                PeriodPotrosnjeOd = ParseDate(periodOdEntry.Text, "Period od mora biti u formatu yyyy-MM-dd."),
                PeriodPotrosnjeDo = ParseDate(periodDoEntry.Text, "Period do mora biti u formatu yyyy-MM-dd."),
                IznosBezPdv = DecimalValue(iznosBezPdvSpin),
                Pdv = DecimalValue(pdvSpin),
                Status = statusCombo.ActiveText,
                NacinPlacanja = nacinPlacanjaCombo.ActiveText,
                Komentar = komentarText.Buffer.Text
            };
        }

        private void Fill(RacunDto racun)
        {
            brojRacunaEntry.Text = racun.BrojRacuna ?? string.Empty;
            potrosacIdEntry.Text = racun.PotrosacId.ToString(CultureInfo.InvariantCulture);
            merenjeIdEntry.Text = racun.MerenjeId.ToString(CultureInfo.InvariantCulture);
            datumIzdavanjaEntry.Text = FormatDate(racun.DatumIzdavanja);
            rokPlacanjaEntry.Text = FormatDate(racun.RokPlacanja);
            periodOdEntry.Text = FormatDate(racun.PeriodPotrosnjeOd);
            periodDoEntry.Text = FormatDate(racun.PeriodPotrosnjeDo);
            iznosBezPdvSpin.Value = Convert.ToDouble(racun.IznosBezPdv);
            pdvSpin.Value = Convert.ToDouble(racun.Pdv);
            SetActiveText(statusCombo, Statusi, racun.Status);
            SetActiveText(nacinPlacanjaCombo, NaciniPlacanja, racun.NacinPlacanja);
            komentarText.Buffer.Text = racun.Komentar ?? string.Empty;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(brojRacunaEntry.Text))
            {
                throw new ArgumentException("Broj racuna je obavezan.");
            }

            ParseLong(potrosacIdEntry.Text, "Potrosac ID mora biti broj.");
            ParseLong(merenjeIdEntry.Text, "Merenje ID mora biti broj.");
            ParseDate(datumIzdavanjaEntry.Text, "Datum izdavanja mora biti u formatu yyyy-MM-dd.");
            ParseDate(rokPlacanjaEntry.Text, "Rok placanja mora biti u formatu yyyy-MM-dd.");
            ParseDate(periodOdEntry.Text, "Period od mora biti u formatu yyyy-MM-dd.");
            ParseDate(periodDoEntry.Text, "Period do mora biti u formatu yyyy-MM-dd.");

            if (string.IsNullOrWhiteSpace(statusCombo.ActiveText))
            {
                throw new ArgumentException("Status racuna je obavezan.");
            }

            if (string.IsNullOrWhiteSpace(nacinPlacanjaCombo.ActiveText))
            {
                throw new ArgumentException("Nacin placanja je obavezan.");
            }
        }

        private static long ParseLong(string value, string message)
        {
            long result;

            if (!long.TryParse(
                value == null ? string.Empty : value.Trim(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out result)
                || result <= 0)
            {
                throw new ArgumentException(message);
            }

            return result;
        }

        private static DateTime ParseDate(string value, string message)
        {
            DateTime result;

            if (!DateTime.TryParseExact(
                value == null ? string.Empty : value.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result))
            {
                throw new ArgumentException(message);
            }

            return result;
        }

        private static decimal DecimalValue(SpinButton spin)
        {
            return Convert.ToDecimal(spin.Value);
        }

        private static string FormatDate(DateTime value)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static void SetActiveText(ComboBoxText combo, string[] values, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            for (var index = 0; index < values.Length; index++)
            {
                if (string.Equals(values[index], value, StringComparison.Ordinal))
                {
                    combo.Active = index;
                    return;
                }
            }

            combo.AppendText(value);
            combo.Active = values.Length;
        }
    }
}
