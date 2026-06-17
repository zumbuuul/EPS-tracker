using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EPS_Tracker
{
    public partial class PoveziPotrosacaBrojiloForma : Form
    {
        private long potrosacId;

        public PoveziPotrosacaBrojiloForma(long id)
        {
            InitializeComponent();

            potrosacId = id;
            txtPotrosacId.Text = id.ToString();
            txtPotrosacId.ReadOnly = true;
        }

        private void btnPovezi_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Klik radi");

            if (string.IsNullOrWhiteSpace(txtSerijskiBroj.Text))
            {
                MessageBox.Show("Unesi serijski broj brojila.");
                return;
            }

            bool uspesno = DTOManager.PoveziPotrosacaIBrojilo(potrosacId, txtSerijskiBroj.Text);

            if (uspesno)
            {
                MessageBox.Show("Potrošač i brojilo su povezani.");
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
