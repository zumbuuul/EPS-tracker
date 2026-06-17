namespace EPS_Tracker
{
    partial class PotrosacDodajForma
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
            cmbTip = new ComboBox();
            txtEmail = new TextBox();
            txtTelefon = new TextBox();
            txtAdresa = new TextBox();
            txtGrad = new TextBox();
            txtStatus = new TextBox();
            btnSacuvaj = new Button();
            txtKategorijaTarife = new TextBox();
            SuspendLayout();
            // 
            // cmbTip
            // 
            cmbTip.FormattingEnabled = true;
            cmbTip.Location = new Point(35, 36);
            cmbTip.Name = "cmbTip";
            cmbTip.Size = new Size(121, 23);
            cmbTip.TabIndex = 0;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(35, 65);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(121, 23);
            txtEmail.TabIndex = 1;
            // 
            // txtTelefon
            // 
            txtTelefon.Location = new Point(35, 94);
            txtTelefon.Name = "txtTelefon";
            txtTelefon.Size = new Size(121, 23);
            txtTelefon.TabIndex = 2;
            // 
            // txtAdresa
            // 
            txtAdresa.Location = new Point(35, 123);
            txtAdresa.Name = "txtAdresa";
            txtAdresa.Size = new Size(121, 23);
            txtAdresa.TabIndex = 3;
            // 
            // txtGrad
            // 
            txtGrad.Location = new Point(35, 152);
            txtGrad.Name = "txtGrad";
            txtGrad.Size = new Size(121, 23);
            txtGrad.TabIndex = 4;
            // 
            // txtStatus
            // 
            txtStatus.Location = new Point(35, 181);
            txtStatus.Name = "txtStatus";
            txtStatus.Size = new Size(121, 23);
            txtStatus.TabIndex = 5;
            // 
            // btnSacuvaj
            // 
            btnSacuvaj.Location = new Point(35, 256);
            btnSacuvaj.Name = "btnSacuvaj";
            btnSacuvaj.Size = new Size(121, 23);
            btnSacuvaj.TabIndex = 6;
            btnSacuvaj.Text = "Sacuvaj";
            btnSacuvaj.UseVisualStyleBackColor = true;
            btnSacuvaj.Click += btnSacuvaj_Click;
            // 
            // txtKategorijaTarife
            // 
            txtKategorijaTarife.Location = new Point(35, 210);
            txtKategorijaTarife.Name = "txtKategorijaTarife";
            txtKategorijaTarife.Size = new Size(121, 23);
            txtKategorijaTarife.TabIndex = 7;
            // 
            // PotrosacDodajForma
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(184, 315);
            Controls.Add(txtKategorijaTarife);
            Controls.Add(btnSacuvaj);
            Controls.Add(txtStatus);
            Controls.Add(txtGrad);
            Controls.Add(txtAdresa);
            Controls.Add(txtTelefon);
            Controls.Add(txtEmail);
            Controls.Add(cmbTip);
            Name = "PotrosacDodajForma";
            Text = "PotrosacDodajForma";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbTip;
        private TextBox txtEmail;
        private TextBox txtTelefon;
        private TextBox txtAdresa;
        private TextBox txtGrad;
        private TextBox txtStatus;
        private Button btnSacuvaj;
        private TextBox txtKategorijaTarife;
    }
}