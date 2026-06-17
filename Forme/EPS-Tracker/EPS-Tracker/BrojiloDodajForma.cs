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
    public partial class BrojiloDodajForma : Form
    {
        public BrojiloDodajForma()
        {
            InitializeComponent();

            txtStatus.Text = "AKTIVNO";
            txtKoeficijentMnozenja.Text = "1";
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSerijskiBroj.Text))
            {
                MessageBox.Show("Unesi serijski broj.");
                return;
            }

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

            BrojiloBasic bb = new BrojiloBasic
            {
                SerijskiBroj = txtSerijskiBroj.Text,
                DatumInstalacije = dtpDatumInstalacije.Value,
                Status = txtStatus.Text,
                Lokacija = txtLokacija.Text,
                KoeficijentMnozenja = koeficijent,
                Komentar = txtKomentar.Text
            };

            bool uspesno = DTOManager.DodajBrojilo(bb);

            if (uspesno)
            {
                MessageBox.Show("Brojilo je dodato.");
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
