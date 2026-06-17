using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static EPS_Tracker.PotrosacPregled;

namespace EPS_Tracker;

public partial class PotrosacIzmeniForma : Form
{
    private PotrosacBasic potrosac;

    public PotrosacIzmeniForma(PotrosacBasic pb)
    {
        InitializeComponent();

        potrosac = pb;

        cmbTip.Items.Add("DOMACINSTVO");
        cmbTip.Items.Add("FIRMA");
        cmbTip.Items.Add("INSTITUCIJA");
        cmbTip.Items.Add("JAVNA_RASVETA");

        cmbTip.SelectedItem = potrosac.Tip;

        txtEmail.Text = potrosac.Email;
        txtTelefon.Text = potrosac.Telefon;
        txtAdresa.Text = potrosac.Adresa;
        txtGrad.Text = potrosac.Grad;
        txtStatus.Text = potrosac.Status;
        txtKategorijaTarife.Text = potrosac.KategorijaTarife;
    }

    private void btnSacuvaj_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Kliknuo sam sacuvaj");

        if (string.IsNullOrWhiteSpace(txtAdresa.Text) ||
            string.IsNullOrWhiteSpace(txtGrad.Text) ||
            string.IsNullOrWhiteSpace(txtStatus.Text) ||
            string.IsNullOrWhiteSpace(txtKategorijaTarife.Text))
        {
            MessageBox.Show("Popuni obavezna polja.");
            return;
        }

        potrosac.Tip = cmbTip.SelectedItem.ToString()!;
        potrosac.Email = txtEmail.Text;
        potrosac.Telefon = txtTelefon.Text;
        potrosac.Adresa = txtAdresa.Text;
        potrosac.Grad = txtGrad.Text;
        potrosac.Status = txtStatus.Text;
        potrosac.KategorijaTarife = txtKategorijaTarife.Text;

        bool uspesno = DTOManager.IzmeniPotrosaca(potrosac);

        if (uspesno)
        {
            MessageBox.Show("Potrošač je izmenjen.");
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
