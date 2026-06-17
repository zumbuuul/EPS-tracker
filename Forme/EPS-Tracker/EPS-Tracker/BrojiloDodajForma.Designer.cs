namespace EPS_Tracker
{
    partial class BrojiloDodajForma
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtSerijskiBroj = new TextBox();
            txtStatus = new TextBox();
            txtLokacija = new TextBox();
            txtKoeficijentMnozenja = new TextBox();
            txtKomentar = new TextBox();
            dtpDatumInstalacije = new DateTimePicker();
            btnSacuvaj = new Button();
            SuspendLayout();
            // 
            // txtSerijskiBroj
            // 
            txtSerijskiBroj.Location = new Point(12, 12);
            txtSerijskiBroj.Name = "txtSerijskiBroj";
            txtSerijskiBroj.Size = new Size(200, 23);
            txtSerijskiBroj.TabIndex = 0;
            // 
            // txtStatus
            // 
            txtStatus.Location = new Point(12, 70);
            txtStatus.Name = "txtStatus";
            txtStatus.Size = new Size(200, 23);
            txtStatus.TabIndex = 1;
            // 
            // txtLokacija
            // 
            txtLokacija.Location = new Point(12, 99);
            txtLokacija.Name = "txtLokacija";
            txtLokacija.Size = new Size(200, 23);
            txtLokacija.TabIndex = 2;
            // 
            // txtKoeficijentMnozenja
            // 
            txtKoeficijentMnozenja.Location = new Point(12, 128);
            txtKoeficijentMnozenja.Name = "txtKoeficijentMnozenja";
            txtKoeficijentMnozenja.Size = new Size(200, 23);
            txtKoeficijentMnozenja.TabIndex = 3;
            // 
            // txtKomentar
            // 
            txtKomentar.Location = new Point(12, 157);
            txtKomentar.Name = "txtKomentar";
            txtKomentar.Size = new Size(200, 23);
            txtKomentar.TabIndex = 4;
            // 
            // dtpDatumInstalacije
            // 
            dtpDatumInstalacije.Location = new Point(12, 41);
            dtpDatumInstalacije.Name = "dtpDatumInstalacije";
            dtpDatumInstalacije.Size = new Size(200, 23);
            dtpDatumInstalacije.TabIndex = 5;
            // 
            // btnSacuvaj
            // 
            btnSacuvaj.Location = new Point(12, 186);
            btnSacuvaj.Name = "btnSacuvaj";
            btnSacuvaj.Size = new Size(200, 23);
            btnSacuvaj.TabIndex = 6;
            btnSacuvaj.Text = "Sacuvaj";
            btnSacuvaj.UseVisualStyleBackColor = true;
            btnSacuvaj.Click += btnSacuvaj_Click;
            // 
            // BrojiloDodajForma
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(232, 225);
            Controls.Add(btnSacuvaj);
            Controls.Add(dtpDatumInstalacije);
            Controls.Add(txtKomentar);
            Controls.Add(txtKoeficijentMnozenja);
            Controls.Add(txtLokacija);
            Controls.Add(txtStatus);
            Controls.Add(txtSerijskiBroj);
            Name = "BrojiloDodajForma";
            Text = "BrojiloDodajForma";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSerijskiBroj;
        private TextBox txtStatus;
        private TextBox txtLokacija;
        private TextBox txtKoeficijentMnozenja;
        private TextBox txtKomentar;
        private DateTimePicker dtpDatumInstalacije;
        private Button btnSacuvaj;
    }
}