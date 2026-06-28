using System;
using System.Globalization;
using app.DTO;
using app.Views.Ui;
using Gtk;

namespace app.Views.Dialogs
{
    public class StanjeDialog : FormDialog
    {
        private readonly long id;
        private readonly Entry datumIVremeEntry;
        private readonly Entry lokacijaEntry;
        private readonly Entry transformatorEntry;
        private readonly SpinButton naponskiNivoSpin;
        private readonly Entry statusEntry;
        private readonly SpinButton proizvedenaEnergijaSpin;
        private readonly SpinButton distribuiranaEnergijaSpin;
        private readonly SpinButton gubitakSpin;
        private readonly SpinButton ukupnaPotrosnjaSpin;
        private readonly TextView komentarText;

        public StanjeDialog(Window parent)
            : this(parent, null)
        {
        }

        public StanjeDialog(Window parent, StanjeDto stanje)
            : base("Stanje mreze", parent)
        {
            id = stanje != null ? stanje.Id : 0;

            AddSection("Lokacija");
            datumIVremeEntry = AddEntry("Datum i vreme", "yyyy-MM-dd HH:mm");
            lokacijaEntry = AddEntry("Lokacija stanice");
            transformatorEntry = AddEntry("Transformator");
            naponskiNivoSpin = AddDecimal("Naponski nivo");
            statusEntry = AddEntry("Status mreze");

            AddSection("Energija");
            proizvedenaEnergijaSpin = AddDecimal("Proizvedena energija");
            distribuiranaEnergijaSpin = AddDecimal("Distribuirana energija");
            gubitakSpin = AddDecimal("Gubici u mrezi");
            ukupnaPotrosnjaSpin = AddDecimal("Ukupna potrosnja");
            komentarText = AddText("Komentar");

            if (stanje != null)
            {
                Fill(stanje);
            }
        }

        public StanjeSaveDto ToSaveDto()
        {
            Validate();

            return new StanjeSaveDto
            {
                Id = id,
                DatumIVreme = ParseDateTime(datumIVremeEntry.Text),
                Lokacija = lokacijaEntry.Text,
                Transformator = transformatorEntry.Text,
                NaponskiNivo = DecimalValue(naponskiNivoSpin),
                Status = statusEntry.Text,
                ProizvedenaEnergija = DecimalValue(proizvedenaEnergijaSpin),
                DistribuiranaEnergija = DecimalValue(distribuiranaEnergijaSpin),
                Gubitak = DecimalValue(gubitakSpin),
                UkupnaPotrosnja = DecimalValue(ukupnaPotrosnjaSpin),
                Komentar = komentarText.Buffer.Text
            };
        }

        private void Fill(StanjeDto stanje)
        {
            datumIVremeEntry.Text = stanje.DatumIVreme.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            lokacijaEntry.Text = stanje.Lokacija ?? string.Empty;
            transformatorEntry.Text = stanje.Transformator ?? string.Empty;
            naponskiNivoSpin.Value = Convert.ToDouble(stanje.NaponskiNivo);
            statusEntry.Text = stanje.Status ?? string.Empty;
            proizvedenaEnergijaSpin.Value = Convert.ToDouble(stanje.ProizvedenaEnergija);
            distribuiranaEnergijaSpin.Value = Convert.ToDouble(stanje.DistribuiranaEnergija);
            gubitakSpin.Value = Convert.ToDouble(stanje.Gubitak);
            ukupnaPotrosnjaSpin.Value = Convert.ToDouble(stanje.UkupnaPotrosnja);
            komentarText.Buffer.Text = stanje.Komentar ?? string.Empty;
        }

        private void Validate()
        {
            ParseDateTime(datumIVremeEntry.Text);

            if (string.IsNullOrWhiteSpace(lokacijaEntry.Text))
            {
                throw new ArgumentException("Lokacija stanice je obavezna.");
            }

            if (string.IsNullOrWhiteSpace(transformatorEntry.Text))
            {
                throw new ArgumentException("Transformator je obavezan.");
            }

            if (string.IsNullOrWhiteSpace(statusEntry.Text))
            {
                throw new ArgumentException("Status mreze je obavezan.");
            }
        }

        private static DateTime ParseDateTime(string value)
        {
            DateTime result;

            if (!DateTime.TryParseExact(
                value == null ? string.Empty : value.Trim(),
                "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result))
            {
                throw new ArgumentException("Datum i vreme moraju biti u formatu yyyy-MM-dd HH:mm.");
            }

            return result;
        }

        private static decimal DecimalValue(SpinButton spin)
        {
            return Convert.ToDecimal(spin.Value);
        }
    }
}
