using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static EPS_Tracker.PotrosacPregled;

namespace EPS_Tracker
{
    public partial class BrojiloIzmeniForma : Form
    {
        private BrojiloBasic brojilo;

        public BrojiloIzmeniForma(BrojiloBasic bb)
        {
            InitializeComponent();

            brojilo = bb;

            txtSerijskiBroj.ReadOnly = true;

            txtSerijskiBroj.Text = brojilo.SerijskiBroj;
            dtpDatumInstalacije.Value = brojilo.DatumInstalacije;
            txtStatus.Text = brojilo.Status;
            txtLokacija.Text = brojilo.Lokacija;
            txtKoeficijentMnozenja.Text = brojilo.KoeficijentMnozenja?.ToString() ?? "";
            txtKomentar.Text = brojilo.Komentar;
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStatus.Text))
            {
                MessageBox.Show("Unesi status.");
                return;
            }

            decimal? koeficijent = null;

            if (!string.IsNullOrWhiteSpace(txtKoeficijentMnozenja.Text))
            {
                if (!decimal.TryParse(txtKoeficijentMnozenja.Text, out decimal broj))
                {
                    MessageBox.Show("Koeficijent množenja mora biti broj.");
                    return;
                }

                koeficijent = broj;
            }

            brojilo.DatumInstalacije = dtpDatumInstalacije.Value;
            brojilo.Status = txtStatus.Text;
            brojilo.Lokacija = txtLokacija.Text;
            brojilo.KoeficijentMnozenja = koeficijent;
            brojilo.Komentar = txtKomentar.Text;

            bool uspesno = DTOManager.IzmeniBrojilo(brojilo);

            if (uspesno)
            {
                MessageBox.Show("Brojilo je izmenjeno.");
                DialogResult = DialogResult.OK;
                Close();
            }
        }

       
    }
}
