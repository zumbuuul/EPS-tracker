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
    public partial class MerenjeDodajForma : Form
    {
        private string serijskiBroj;

        public MerenjeDodajForma(string serijski)
        {
            InitializeComponent();

            serijskiBroj = serijski;
            txtSerijskiBroj.Text = serijski;
            txtSerijskiBroj.ReadOnly = true;

            txtIsValidirano.Text = "N";
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIsValidirano.Text))
            {
                MessageBox.Show("Unesi da li je validirano: D ili N.");
                return;
            }

            decimal? potrosnjaAktivna = ParseDecimal(txtPotrosnjaAktivna.Text, "Aktivna potrošnja");
            decimal? potrosnjaReaktivna = ParseDecimal(txtPotrosnjaReaktivna.Text, "Reaktivna potrošnja");
            decimal? snaga = ParseDecimal(txtSnaga.Text, "Snaga");
            decimal? napon = ParseDecimal(txtNapon.Text, "Napon");
            decimal? struja = ParseDecimal(txtStruja.Text, "Struja");

            if (parseGreska)
            {
                return;
            }

            MerenjeBasic mb = new MerenjeBasic
            {
                SerijskiBroj = serijskiBroj,
                DatumVremeMerenja = dtpDatumVremeMerenja.Value,
                PotrosnjaAktivna = potrosnjaAktivna,
                PotrosnjaReaktivna = potrosnjaReaktivna,
                Snaga = snaga,
                Napon = napon,
                Struja = struja,
                TipMerenja = txtTipMerenja.Text,
                TipIzvora = txtTipIzvora.Text,
                IsValidirano = txtIsValidirano.Text,
                Komentar = txtKomentar.Text
            };

            bool uspesno = DTOManager.DodajMerenje(mb);

            if (uspesno)
            {
                MessageBox.Show("Merenje je dodato.");
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool parseGreska = false;

        private decimal? ParseDecimal(string text, string nazivPolja)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (!decimal.TryParse(text, out decimal vrednost))
            {
                MessageBox.Show(nazivPolja + " mora biti broj.");
                parseGreska = true;
                return null;
            }

            return vrednost;
        }

        
    }
}
