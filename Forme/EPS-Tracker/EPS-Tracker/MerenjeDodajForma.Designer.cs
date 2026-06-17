namespace EPS_Tracker
{
    partial class MerenjeDodajForma
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
            dtpDatumVremeMerenja = new DateTimePicker();
            txtPotrosnjaAktivna = new TextBox();
            txtPotrosnjaReaktivna = new TextBox();
            txtSnaga = new TextBox();
            txtNapon = new TextBox();
            txtStruja = new TextBox();
            txtTipMerenja = new TextBox();
            txtIsValidirano = new TextBox();
            txtTipIzvora = new TextBox();
            txtKomentar = new TextBox();
            btnSacuvaj = new Button();
            SuspendLayout();
            // 
            // txtSerijskiBroj
            // 
            txtSerijskiBroj.Location = new Point(12, 12);
            txtSerijskiBroj.Name = "txtSerijskiBroj";
            txtSerijskiBroj.ReadOnly = true;
            txtSerijskiBroj.Size = new Size(200, 23);
            txtSerijskiBroj.TabIndex = 0;
            // 
            // dtpDatumVremeMerenja
            // 
            dtpDatumVremeMerenja.Location = new Point(12, 41);
            dtpDatumVremeMerenja.Name = "dtpDatumVremeMerenja";
            dtpDatumVremeMerenja.Size = new Size(200, 23);
            dtpDatumVremeMerenja.TabIndex = 1;
            // 
            // txtPotrosnjaAktivna
            // 
            txtPotrosnjaAktivna.Location = new Point(12, 70);
            txtPotrosnjaAktivna.Name = "txtPotrosnjaAktivna";
            txtPotrosnjaAktivna.Size = new Size(200, 23);
            txtPotrosnjaAktivna.TabIndex = 2;
            // 
            // txtPotrosnjaReaktivna
            // 
            txtPotrosnjaReaktivna.Location = new Point(12, 99);
            txtPotrosnjaReaktivna.Name = "txtPotrosnjaReaktivna";
            txtPotrosnjaReaktivna.Size = new Size(200, 23);
            txtPotrosnjaReaktivna.TabIndex = 3;
            // 
            // txtSnaga
            // 
            txtSnaga.Location = new Point(12, 128);
            txtSnaga.Name = "txtSnaga";
            txtSnaga.Size = new Size(200, 23);
            txtSnaga.TabIndex = 4;
            // 
            // txtNapon
            // 
            txtNapon.Location = new Point(12, 157);
            txtNapon.Name = "txtNapon";
            txtNapon.Size = new Size(200, 23);
            txtNapon.TabIndex = 5;
            // 
            // txtStruja
            // 
            txtStruja.Location = new Point(12, 186);
            txtStruja.Name = "txtStruja";
            txtStruja.Size = new Size(200, 23);
            txtStruja.TabIndex = 6;
            // 
            // txtTipMerenja
            // 
            txtTipMerenja.Location = new Point(12, 215);
            txtTipMerenja.Name = "txtTipMerenja";
            txtTipMerenja.Size = new Size(200, 23);
            txtTipMerenja.TabIndex = 7;
            // 
            // txtIsValidirano
            // 
            txtIsValidirano.Location = new Point(12, 273);
            txtIsValidirano.Name = "txtIsValidirano";
            txtIsValidirano.Size = new Size(200, 23);
            txtIsValidirano.TabIndex = 8;
            // 
            // txtTipIzvora
            // 
            txtTipIzvora.Location = new Point(12, 244);
            txtTipIzvora.Name = "txtTipIzvora";
            txtTipIzvora.Size = new Size(200, 23);
            txtTipIzvora.TabIndex = 9;
            // 
            // txtKomentar
            // 
            txtKomentar.Location = new Point(12, 302);
            txtKomentar.Name = "txtKomentar";
            txtKomentar.Size = new Size(200, 23);
            txtKomentar.TabIndex = 10;
            // 
            // btnSacuvaj
            // 
            btnSacuvaj.Location = new Point(12, 331);
            btnSacuvaj.Name = "btnSacuvaj";
            btnSacuvaj.Size = new Size(200, 23);
            btnSacuvaj.TabIndex = 11;
            btnSacuvaj.Text = "Sacuvaj";
            btnSacuvaj.UseVisualStyleBackColor = true;
            btnSacuvaj.Click += btnSacuvaj_Click;
            // 
            // MerenjeDodajForma
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(241, 367);
            Controls.Add(btnSacuvaj);
            Controls.Add(txtKomentar);
            Controls.Add(txtTipIzvora);
            Controls.Add(txtIsValidirano);
            Controls.Add(txtTipMerenja);
            Controls.Add(txtStruja);
            Controls.Add(txtNapon);
            Controls.Add(txtSnaga);
            Controls.Add(txtPotrosnjaReaktivna);
            Controls.Add(txtPotrosnjaAktivna);
            Controls.Add(dtpDatumVremeMerenja);
            Controls.Add(txtSerijskiBroj);
            Name = "MerenjeDodajForma";
            Text = "MerenjeDodajForma";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSerijskiBroj;
        private DateTimePicker dtpDatumVremeMerenja;
        private TextBox txtPotrosnjaAktivna;
        private TextBox txtPotrosnjaReaktivna;
        private TextBox txtSnaga;
        private TextBox txtNapon;
        private TextBox txtStruja;
        private TextBox txtTipMerenja;
        private TextBox txtIsValidirano;
        private TextBox txtTipIzvora;
        private TextBox txtKomentar;
        private Button btnSacuvaj;
    }
}