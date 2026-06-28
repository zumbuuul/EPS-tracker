using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using app.DTO;
using app.Entities.Enums;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class BrojiloDialog : FormDialog
    {
        private static readonly string[] ProtokolNames = new[] { string.Empty, "GSM", "GPRS" };
        private static readonly string[] DaNeNames = Enum.GetNames(typeof(DaNe));

        private readonly Entry serijskiBrojEntry;
        private readonly Entry datumInstalacijeEntry;
        private readonly Entry datumiZameneEntry;
        private readonly Entry statusEntry;
        private readonly Entry lokacijaEntry;
        private readonly SpinButton koeficijentSpin;
        private readonly TextView komentarText;

        private readonly CheckButton jednofaznoCheck;
        private readonly CheckButton trofaznoCheck;
        private readonly CheckButton pametnoCheck;
        private readonly CheckButton mehanickoCheck;
        private readonly CheckButton rasvetnoCheck;

        private readonly Label mehanickoSection;
        private readonly Entry poslednjaKalibracijaEntry;
        private readonly Entry preciznostEntry;
        private readonly SpinButton maxGreskaSpin;

        private readonly Label pametnoSection;
        private readonly ComboBoxText protokolCombo;
        private readonly SpinButton frekvencijaSpin;
        private readonly ComboBoxText daljinskoIskljucenjeCombo;
        private readonly SpinButton nivoBaterijeSpin;

        private readonly Label trofaznoSection;
        private readonly SpinButton maxSnagaSpin;
        private readonly ComboBoxText merenjePoZonamaCombo;
        private readonly SpinButton ugovorenaSnagaSpin;

        private readonly Label rasvetnoSection;
        private readonly Entry vremeUkljucenjaEntry;
        private readonly Entry vremeIskljucenjaEntry;
        private readonly Entry statusSenzoraEntry;
        private readonly SpinButton radnoVremeSpin;

        public BrojiloDialog(Window parent)
            : this(parent, null)
        {
        }

        public BrojiloDialog(Window parent, BrojiloDto brojilo)
            : base("Brojilo", parent)
        {
            AddSection("Osnovni podaci");
            serijskiBrojEntry = AddEntry("Serijski broj");
            datumInstalacijeEntry = AddEntry("Datum instalacije", "yyyy-MM-dd");
            datumiZameneEntry = AddEntry("Datumi zamene", "yyyy-MM-dd, yyyy-MM-dd");
            statusEntry = AddEntry("Status");
            lokacijaEntry = AddEntry("Lokacija");
            koeficijentSpin = AddDecimal("Koeficijent mnozenja");
            komentarText = AddText("Komentar");

            AddSection("Tipovi brojila");
            jednofaznoCheck = AddCheck("JEDNOFAZNO");
            trofaznoCheck = AddCheck("TROFAZNO");
            pametnoCheck = AddCheck("PAMETNO");
            mehanickoCheck = AddCheck("MEHANICKO");
            rasvetnoCheck = AddCheck("RASVETNO");

            mehanickoSection = AddSection("Mehanicko brojilo");
            poslednjaKalibracijaEntry = AddEntry("Poslednja kalibracija", "yyyy-MM-dd");
            preciznostEntry = AddEntry("Preciznost klase");
            maxGreskaSpin = AddDecimal("Maksimalna greska");

            pametnoSection = AddSection("Pametno brojilo");
            protokolCombo = AddCombo("Protokol", ProtokolNames);
            frekvencijaSpin = AddDecimal("Frekvencija slanja");
            daljinskoIskljucenjeCombo = AddCombo("Daljinsko iskljucenje", DaNeNames);
            nivoBaterijeSpin = AddDecimal("Nivo baterije");

            trofaznoSection = AddSection("Trofazno brojilo");
            maxSnagaSpin = AddDecimal("Maksimalna snaga");
            merenjePoZonamaCombo = AddCombo("Merenje po zonama", DaNeNames);
            ugovorenaSnagaSpin = AddDecimal("Ugovorena snaga");

            rasvetnoSection = AddSection("Javna rasveta");
            vremeUkljucenjaEntry = AddEntry("Vreme ukljucenja");
            vremeIskljucenjaEntry = AddEntry("Vreme iskljucenja");
            statusSenzoraEntry = AddEntry("Status fotosenzora");
            radnoVremeSpin = AddDecimal("Nocno vreme rada");

            jednofaznoCheck.Toggled += TipToggled;
            trofaznoCheck.Toggled += TipToggled;
            pametnoCheck.Toggled += TipToggled;
            mehanickoCheck.Toggled += TipToggled;
            rasvetnoCheck.Toggled += TipToggled;

            if (brojilo != null)
            {
                Fill(brojilo);
                serijskiBrojEntry.Sensitive = false;
            }

            UpdateConditionalSections();
        }

        public BrojiloSaveDto ToSaveDto()
        {
            Validate();
            var tipovi = ActiveTipovi();

            return new BrojiloSaveDto
            {
                SerijskiBroj = serijskiBrojEntry.Text,
                DatumInstalacije = ParseRequiredDate(datumInstalacijeEntry.Text, "Datum instalacije mora biti u formatu yyyy-MM-dd."),
                Status = statusEntry.Text,
                Lokacija = lokacijaEntry.Text,
                KoeficijentMnozenja = OptionalDecimal(koeficijentSpin),
                Komentar = komentarText.Buffer.Text,
                TipoviBrojila = tipovi,
                DatumiZamene = ParseOptionalDates(datumiZameneEntry.Text),
                Mehanicko = tipovi.Contains("MEHANICKO") ? BuildMehanickoDto() : null,
                Pametno = tipovi.Contains("PAMETNO") ? BuildPametnoDto() : null,
                Trofazno = tipovi.Contains("TROFAZNO") ? BuildTrofaznoDto() : null,
                Rasvetno = tipovi.Contains("RASVETNO") ? BuildRasvetnoDto() : null
            };
        }

        private void TipToggled(object sender, EventArgs args)
        {
            if (sender == pametnoCheck && pametnoCheck.Active)
            {
                mehanickoCheck.Active = false;
            }

            if (sender == mehanickoCheck && mehanickoCheck.Active)
            {
                pametnoCheck.Active = false;
            }

            if (sender == jednofaznoCheck && jednofaznoCheck.Active)
            {
                trofaznoCheck.Active = false;
            }

            if (sender == trofaznoCheck && trofaznoCheck.Active)
            {
                jednofaznoCheck.Active = false;
            }

            UpdateConditionalSections();
        }

        private void UpdateConditionalSections()
        {
            SetWidgetVisible(mehanickoSection, mehanickoCheck.Active);
            SetRowVisible(poslednjaKalibracijaEntry, mehanickoCheck.Active);
            SetRowVisible(preciznostEntry, mehanickoCheck.Active);
            SetRowVisible(maxGreskaSpin, mehanickoCheck.Active);

            SetWidgetVisible(pametnoSection, pametnoCheck.Active);
            SetRowVisible(protokolCombo, pametnoCheck.Active);
            SetRowVisible(frekvencijaSpin, pametnoCheck.Active);
            SetRowVisible(daljinskoIskljucenjeCombo, pametnoCheck.Active);
            SetRowVisible(nivoBaterijeSpin, pametnoCheck.Active);

            SetWidgetVisible(trofaznoSection, trofaznoCheck.Active);
            SetRowVisible(maxSnagaSpin, trofaznoCheck.Active);
            SetRowVisible(merenjePoZonamaCombo, trofaznoCheck.Active);
            SetRowVisible(ugovorenaSnagaSpin, trofaznoCheck.Active);

            SetWidgetVisible(rasvetnoSection, rasvetnoCheck.Active);
            SetRowVisible(vremeUkljucenjaEntry, rasvetnoCheck.Active);
            SetRowVisible(vremeIskljucenjaEntry, rasvetnoCheck.Active);
            SetRowVisible(statusSenzoraEntry, rasvetnoCheck.Active);
            SetRowVisible(radnoVremeSpin, rasvetnoCheck.Active);
        }

        private void Fill(BrojiloDto brojilo)
        {
            serijskiBrojEntry.Text = brojilo.SerijskiBroj ?? string.Empty;
            datumInstalacijeEntry.Text = FormatDate(brojilo.DatumInstalacije);
            datumiZameneEntry.Text = string.Join(", ", brojilo.DatumiZamene.Select(FormatDate));
            statusEntry.Text = brojilo.Status ?? string.Empty;
            lokacijaEntry.Text = brojilo.Lokacija ?? string.Empty;
            SetDecimal(koeficijentSpin, brojilo.KoeficijentMnozenja);
            komentarText.Buffer.Text = brojilo.Komentar ?? string.Empty;

            SetTipovi(brojilo.TipoviBrojila);
            FillMehanicko(brojilo.Mehanicko);
            FillPametno(brojilo.Pametno);
            FillTrofazno(brojilo.Trofazno);
            FillRasvetno(brojilo.Rasvetno);
        }

        private void FillMehanicko(MehanickoBrojiloDto mehanicko)
        {
            if (mehanicko == null)
            {
                return;
            }

            poslednjaKalibracijaEntry.Text = mehanicko.PoslednjaKalibracija.HasValue
                ? FormatDate(mehanicko.PoslednjaKalibracija.Value)
                : string.Empty;
            preciznostEntry.Text = mehanicko.Preciznost ?? string.Empty;
            SetDecimal(maxGreskaSpin, mehanicko.MaxGreska);
        }

        private void FillPametno(PametnoBrojiloDto pametno)
        {
            if (pametno == null)
            {
                return;
            }

            SetActiveText(protokolCombo, pametno.Protokol, ProtokolNames);
            SetDecimal(frekvencijaSpin, pametno.Frekvencija);
            SetActiveText(daljinskoIskljucenjeCombo, pametno.IsDaljinskoIskljucenje, DaNeNames);
            SetDecimal(nivoBaterijeSpin, pametno.NivoBaterije);
        }

        private void FillTrofazno(TrofaznoBrojiloDto trofazno)
        {
            if (trofazno == null)
            {
                return;
            }

            SetDecimal(maxSnagaSpin, trofazno.MaxSnaga);
            SetActiveText(merenjePoZonamaCombo, trofazno.MogucnostMerenjaPoZonama, DaNeNames);
            SetDecimal(ugovorenaSnagaSpin, trofazno.UgovorenaSnaga);
        }

        private void FillRasvetno(RasvetnoBrojiloDto rasvetno)
        {
            if (rasvetno == null)
            {
                return;
            }

            vremeUkljucenjaEntry.Text = rasvetno.VremeUkljucenja ?? string.Empty;
            vremeIskljucenjaEntry.Text = rasvetno.VremeIskljucenja ?? string.Empty;
            statusSenzoraEntry.Text = rasvetno.StatusSenzora ?? string.Empty;
            SetDecimal(radnoVremeSpin, rasvetno.RadnoVreme);
        }

        private MehanickoBrojiloDto BuildMehanickoDto()
        {
            return new MehanickoBrojiloDto
            {
                PoslednjaKalibracija = ParseOptionalDate(poslednjaKalibracijaEntry.Text, "Poslednja kalibracija mora biti u formatu yyyy-MM-dd."),
                Preciznost = preciznostEntry.Text,
                MaxGreska = OptionalDecimal(maxGreskaSpin)
            };
        }

        private PametnoBrojiloDto BuildPametnoDto()
        {
            return new PametnoBrojiloDto
            {
                Protokol = protokolCombo.ActiveText,
                Frekvencija = OptionalDecimal(frekvencijaSpin),
                IsDaljinskoIskljucenje = daljinskoIskljucenjeCombo.ActiveText,
                NivoBaterije = OptionalDecimal(nivoBaterijeSpin)
            };
        }

        private TrofaznoBrojiloDto BuildTrofaznoDto()
        {
            return new TrofaznoBrojiloDto
            {
                MaxSnaga = OptionalDecimal(maxSnagaSpin),
                MogucnostMerenjaPoZonama = merenjePoZonamaCombo.ActiveText,
                UgovorenaSnaga = OptionalDecimal(ugovorenaSnagaSpin)
            };
        }

        private RasvetnoBrojiloDto BuildRasvetnoDto()
        {
            return new RasvetnoBrojiloDto
            {
                VremeUkljucenja = vremeUkljucenjaEntry.Text,
                VremeIskljucenja = vremeIskljucenjaEntry.Text,
                StatusSenzora = statusSenzoraEntry.Text,
                RadnoVreme = OptionalDecimal(radnoVremeSpin)
            };
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(serijskiBrojEntry.Text))
            {
                throw new ArgumentException("Serijski broj je obavezan.");
            }

            ParseRequiredDate(datumInstalacijeEntry.Text, "Datum instalacije mora biti u formatu yyyy-MM-dd.");
            ParseOptionalDates(datumiZameneEntry.Text);

            if (string.IsNullOrWhiteSpace(statusEntry.Text))
            {
                throw new ArgumentException("Status brojila je obavezan.");
            }

            var tipovi = ActiveTipovi();

            if (tipovi.Count == 0)
            {
                throw new ArgumentException("Izaberi bar jedan tip brojila.");
            }

            if (tipovi.Contains("PAMETNO") && tipovi.Contains("MEHANICKO"))
            {
                throw new ArgumentException("Brojilo ne moze biti i PAMETNO i MEHANICKO.");
            }

            if (tipovi.Contains("JEDNOFAZNO") && tipovi.Contains("TROFAZNO"))
            {
                throw new ArgumentException("Brojilo ne moze biti i JEDNOFAZNO i TROFAZNO.");
            }

            if (mehanickoCheck.Active)
            {
                ParseOptionalDate(poslednjaKalibracijaEntry.Text, "Poslednja kalibracija mora biti u formatu yyyy-MM-dd.");
            }
        }

        private IList<string> ActiveTipovi()
        {
            var tipovi = new List<string>();

            if (jednofaznoCheck.Active)
            {
                tipovi.Add(TipBrojila.JEDNOFAZNO.ToString());
            }

            if (trofaznoCheck.Active)
            {
                tipovi.Add(TipBrojila.TROFAZNO.ToString());
            }

            if (pametnoCheck.Active)
            {
                tipovi.Add(TipBrojila.PAMETNO.ToString());
            }

            if (mehanickoCheck.Active)
            {
                tipovi.Add(TipBrojila.MEHANICKO.ToString());
            }

            if (rasvetnoCheck.Active)
            {
                tipovi.Add(TipBrojila.RASVETNO.ToString());
            }

            return tipovi;
        }

        private void SetTipovi(IList<string> tipovi)
        {
            jednofaznoCheck.Active = HasTip(tipovi, TipBrojila.JEDNOFAZNO);
            trofaznoCheck.Active = HasTip(tipovi, TipBrojila.TROFAZNO);
            pametnoCheck.Active = HasTip(tipovi, TipBrojila.PAMETNO);
            mehanickoCheck.Active = HasTip(tipovi, TipBrojila.MEHANICKO);
            rasvetnoCheck.Active = HasTip(tipovi, TipBrojila.RASVETNO);
        }

        private static bool HasTip(IList<string> tipovi, TipBrojila tip)
        {
            return tipovi != null && tipovi.Contains(tip.ToString());
        }

        private static IList<DateTime> ParseOptionalDates(string value)
        {
            var dates = new List<DateTime>();

            if (string.IsNullOrWhiteSpace(value))
            {
                return dates;
            }

            foreach (var part in value.Split(','))
            {
                dates.Add(ParseRequiredDate(part, "Datumi zamene moraju biti u formatu yyyy-MM-dd."));
            }

            return dates;
        }

        private static DateTime? ParseOptionalDate(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return ParseRequiredDate(value, message);
        }

        private static DateTime ParseRequiredDate(string value, string message)
        {
            DateTime date;

            if (!DateTime.TryParseExact(
                value == null ? string.Empty : value.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date))
            {
                throw new ArgumentException(message);
            }

            return date;
        }

        private static void SetActiveText(ComboBoxText combo, string value, string[] values)
        {
            for (var index = 0; index < values.Length; index++)
            {
                if (values[index] == (value ?? string.Empty))
                {
                    combo.Active = index;
                    return;
                }
            }
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

        private static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
