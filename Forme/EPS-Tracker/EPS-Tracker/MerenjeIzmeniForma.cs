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
    public partial class MerenjeIzmeniForma : Form
    {
        private MerenjeBasic merenje;
        private bool parseGreska = false;

        public MerenjeIzmeniForma(MerenjeBasic mb)
        {
            InitializeComponent();

            merenje = mb;

            txtSerijskiBroj.ReadOnly = true;

            txtSerijskiBroj.Text = merenje.SerijskiBroj;
            dtpDatumVremeMerenja.Value = merenje.DatumVremeMerenja;
            txtPotrosnjaAktivna.Text = merenje.PotrosnjaAktivna?.ToString() ?? "";
            txtPotrosnjaReaktivna.Text = merenje.PotrosnjaReaktivna?.ToString() ?? "";
            txtSnaga.Text = merenje.Snaga?.ToString() ?? "";
            txtNapon.Text = merenje.Napon?.ToString() ?? "";
            txtStruja.Text = merenje.Struja?.ToString() ?? "";
            txtTipMerenja.Text = merenje.TipMerenja;
            txtTipIzvora.Text = merenje.TipIzvora;
            txtIsValidirano.Text = merenje.IsValidirano;
            txtKomentar.Text = merenje.Komentar;
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            parseGreska = false;

            if (string.IsNullOrWhiteSpace(txtIsValidirano.Text))
            {
                MessageBox.Show("Unesi da li je validirano: D ili N.");
                return;
            }

            if (txtIsValidirano.Text != "D" && txtIsValidirano.Text != "N")
            {
                MessageBox.Show("IsValidirano mora biti D ili N.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtTipIzvora.Text) &&
                txtTipIzvora.Text != "RUCNO_OCITAVANJE" &&
                txtTipIzvora.Text != "PAMETNO_BROJILO")
            {
                MessageBox.Show("Tip izvora mora biti RUCNO_OCITAVANJE ili PAMETNO_BROJILO.");
                return;
            }

            merenje.DatumVremeMerenja = dtpDatumVremeMerenja.Value;
            merenje.PotrosnjaAktivna = ParseDecimal(txtPotrosnjaAktivna.Text, "Aktivna potrošnja");
            merenje.PotrosnjaReaktivna = ParseDecimal(txtPotrosnjaReaktivna.Text, "Reaktivna potrošnja");
            merenje.Snaga = ParseDecimal(txtSnaga.Text, "Snaga");
            merenje.Napon = ParseDecimal(txtNapon.Text, "Napon");
            merenje.Struja = ParseDecimal(txtStruja.Text, "Struja");

            if (parseGreska)
            {
                return;
            }

            merenje.TipMerenja = txtTipMerenja.Text;
            merenje.TipIzvora = txtTipIzvora.Text;
            merenje.IsValidirano = txtIsValidirano.Text;
            merenje.Komentar = txtKomentar.Text;

            bool uspesno = DTOManager.IzmeniMerenje(merenje);

            if (uspesno)
            {
                MessageBox.Show("Merenje je izmenjeno.");
                DialogResult = DialogResult.OK;
                Close();
            }
        }

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
