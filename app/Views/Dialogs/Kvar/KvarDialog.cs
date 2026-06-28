using System;
using System.Globalization;
using app.DTO;
using app.Entities.Enums;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class KvarDialog : FormDialog
    {
        private static readonly string[] StatusNames = Enum.GetNames(typeof(KvarStatus));
        private static readonly string[] PrioritetNames = new[]
        {
            string.Empty,
            "NIZAK",
            "SREDNJI",
            "VISOK",
            "KRITICAN"
        };

        private readonly long id;
        private readonly Entry serijskiBrojEntry;
        private readonly Entry potrosacIdEntry;
        private readonly Entry datumPrijaveEntry;
        private readonly Entry tipKvaraEntry;
        private readonly ComboBoxText statusCombo;
        private readonly ComboBoxText prioritetCombo;
        private readonly Entry nadlezniTimEntry;
        private readonly Entry datumOtklanjanjaEntry;
        private readonly SpinButton trajanjeSpin;
        private readonly TextView opisProblemaText;
        private readonly TextView komentarText;

        public KvarDialog(Window parent)
            : this(parent, null)
        {
        }

        public KvarDialog(Window parent, KvarDto kvar)
            : base("Kvar", parent)
        {
            id = kvar != null ? kvar.Id : 0;

            AddSection("Veze");
            serijskiBrojEntry = AddEntry("Serijski broj brojila");
            potrosacIdEntry = AddEntry("Potrosac ID");

            AddSection("Prijava");
            datumPrijaveEntry = AddEntry("Datum prijave", "yyyy-MM-dd HH:mm");
            tipKvaraEntry = AddEntry("Tip kvara");
            statusCombo = AddCombo("Status", StatusNames);
            prioritetCombo = AddCombo("Prioritet", PrioritetNames);
            nadlezniTimEntry = AddEntry("Nadlezni tim");

            AddSection("Otklanjanje");
            datumOtklanjanjaEntry = AddEntry("Datum otklanjanja", "yyyy-MM-dd HH:mm");
            trajanjeSpin = AddDecimal("Trajanje u satima");
            opisProblemaText = AddText("Opis problema");
            komentarText = AddText("Komentar");

            if (kvar != null)
            {
                Fill(kvar);
            }
        }

        public KvarSaveDto ToSaveDto()
        {
            Validate();

            return new KvarSaveDto
            {
                Id = id,
                SerijskiBroj = serijskiBrojEntry.Text,
                PotrosacId = ParseLong(potrosacIdEntry.Text, "Potrosac ID mora biti broj."),
                DatumPrijave = ParseRequiredDateTime(datumPrijaveEntry.Text, "Datum prijave mora biti u formatu yyyy-MM-dd HH:mm."),
                TipKvara = tipKvaraEntry.Text,
                OpisProblema = opisProblemaText.Buffer.Text,
                Status = statusCombo.ActiveText,
                DatumOtklanjanja = ParseOptionalDateTime(datumOtklanjanjaEntry.Text, "Datum otklanjanja mora biti u formatu yyyy-MM-dd HH:mm."),
                TrajanjeUSatima = OptionalDecimal(trajanjeSpin),
                Prioritet = prioritetCombo.ActiveText,
                NadlezniTim = nadlezniTimEntry.Text,
                Komentar = komentarText.Buffer.Text
            };
        }

        private void Fill(KvarDto kvar)
        {
            serijskiBrojEntry.Text = kvar.SerijskiBroj ?? string.Empty;
            potrosacIdEntry.Text = kvar.PotrosacId.ToString(CultureInfo.InvariantCulture);
            datumPrijaveEntry.Text = FormatDateTime(kvar.DatumPrijave);
            tipKvaraEntry.Text = kvar.TipKvara ?? string.Empty;
            SetActiveText(statusCombo, StatusNames, kvar.Status);
            SetActiveText(prioritetCombo, PrioritetNames, kvar.Prioritet);
            nadlezniTimEntry.Text = kvar.NadlezniTim ?? string.Empty;
            datumOtklanjanjaEntry.Text = kvar.DatumOtklanjanja.HasValue
                ? FormatDateTime(kvar.DatumOtklanjanja.Value)
                : string.Empty;
            SetDecimal(trajanjeSpin, kvar.TrajanjeUSatima);
            opisProblemaText.Buffer.Text = kvar.OpisProblema ?? string.Empty;
            komentarText.Buffer.Text = kvar.Komentar ?? string.Empty;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(serijskiBrojEntry.Text))
            {
                throw new ArgumentException("Serijski broj brojila je obavezan.");
            }

            ParseLong(potrosacIdEntry.Text, "Potrosac ID mora biti broj.");
            ParseRequiredDateTime(datumPrijaveEntry.Text, "Datum prijave mora biti u formatu yyyy-MM-dd HH:mm.");

            if (string.IsNullOrWhiteSpace(tipKvaraEntry.Text))
            {
                throw new ArgumentException("Tip kvara je obavezan.");
            }

            if (string.IsNullOrWhiteSpace(statusCombo.ActiveText))
            {
                throw new ArgumentException("Status kvara je obavezan.");
            }

            ParseOptionalDateTime(datumOtklanjanjaEntry.Text, "Datum otklanjanja mora biti u formatu yyyy-MM-dd HH:mm.");
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

        private static DateTime ParseRequiredDateTime(string value, string message)
        {
            DateTime result;

            if (!DateTime.TryParseExact(
                value == null ? string.Empty : value.Trim(),
                "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result))
            {
                throw new ArgumentException(message);
            }

            return result;
        }

        private static DateTime? ParseOptionalDateTime(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return ParseRequiredDateTime(value, message);
        }

        private static decimal? OptionalDecimal(SpinButton spin)
        {
            var value = Convert.ToDecimal(spin.Value);
            return value == 0 ? (decimal?)null : value;
        }

        private static void SetDecimal(SpinButton spin, decimal? value)
        {
            spin.Value = value.HasValue ? Convert.ToDouble(value.Value) : 0;
        }

        private static string FormatDateTime(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }

        private static void SetActiveText(ComboBoxText combo, string[] values, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            for (var index = 0; index < values.Length; index++)
            {
                if (values[index] == value)
                {
                    combo.Active = index;
                    return;
                }
            }
        }
    }
}
