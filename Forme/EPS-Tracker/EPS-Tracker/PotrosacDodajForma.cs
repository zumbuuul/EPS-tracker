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
    public partial class PotrosacDodajForma : Form
    {
        public PotrosacDodajForma()
        {
            InitializeComponent();

            cmbTip.Items.Add("DOMACINSTVO");
            cmbTip.Items.Add("FIRMA");
            cmbTip.Items.Add("INSTITUCIJA");
            cmbTip.Items.Add("JAVNA_RASVETA");

            cmbTip.SelectedIndex = 0;

            txtStatus.Text = "AKTIVAN";
            txtKategorijaTarife.Text = "PLAVA";
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtAdresa.Text) ||
                string.IsNullOrWhiteSpace(txtGrad.Text) ||
                string.IsNullOrWhiteSpace(txtStatus.Text) ||
                string.IsNullOrWhiteSpace(txtKategorijaTarife.Text))
            {
                MessageBox.Show("Popuni obavezna polja.");
                return;
            }

            PotrosacBasic pb = new PotrosacBasic
            {
                Tip = cmbTip.SelectedItem.ToString()!,
                Email = txtEmail.Text,
                Telefon = txtTelefon.Text,
                Adresa = txtAdresa.Text,
                Grad = txtGrad.Text,
                Status = txtStatus.Text,
                KategorijaTarife = txtKategorijaTarife.Text
            };

            bool uspesno = DTOManager.DodajPotrosaca(pb);

            if (uspesno)
            {
                MessageBox.Show("Potrošač je dodat.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
