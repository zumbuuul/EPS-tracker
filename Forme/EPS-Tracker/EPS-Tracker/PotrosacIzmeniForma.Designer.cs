namespace EPS_Tracker
{
    partial class PotrosacIzmeniForma
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
            txtKategorijaTarife = new TextBox();
            btnSacuvaj = new Button();
            SuspendLayout();
            // 
            // cmbTip
            // 
            cmbTip.FormattingEnabled = true;
            cmbTip.Location = new Point(12, 12);
            cmbTip.Name = "cmbTip";
            cmbTip.Size = new Size(121, 23);
            cmbTip.TabIndex = 0;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(12, 41);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(121, 23);
            txtEmail.TabIndex = 1;
            // 
            // txtTelefon
            // 
            txtTelefon.Location = new Point(12, 70);
            txtTelefon.Name = "txtTelefon";
            txtTelefon.Size = new Size(121, 23);
            txtTelefon.TabIndex = 2;
            // 
            // txtAdresa
            // 
            txtAdresa.Location = new Point(12, 99);
            txtAdresa.Name = "txtAdresa";
            txtAdresa.Size = new Size(121, 23);
            txtAdresa.TabIndex = 3;
            // 
            // txtGrad
            // 
            txtGrad.Location = new Point(12, 128);
            txtGrad.Name = "txtGrad";
            txtGrad.Size = new Size(121, 23);
            txtGrad.TabIndex = 4;
            // 
            // txtStatus
            // 
            txtStatus.Location = new Point(12, 157);
            txtStatus.Name = "txtStatus";
            txtStatus.Size = new Size(121, 23);
            txtStatus.TabIndex = 5;
            // 
            // txtKategorijaTarife
            // 
            txtKategorijaTarife.Location = new Point(12, 186);
            txtKategorijaTarife.Name = "txtKategorijaTarife";
            txtKategorijaTarife.Size = new Size(121, 23);
            txtKategorijaTarife.TabIndex = 6;
            // 
            // btnSacuvaj
            // 
            btnSacuvaj.Location = new Point(12, 215);
            btnSacuvaj.Name = "btnSacuvaj";
            btnSacuvaj.Size = new Size(121, 23);
            btnSacuvaj.TabIndex = 7;
            btnSacuvaj.Text = "Sacuvaj";
            btnSacuvaj.UseVisualStyleBackColor = true;
            btnSacuvaj.Click += btnSacuvaj_Click;
            // 
            // PotrosacIzmeniForma
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(165, 267);
            Controls.Add(btnSacuvaj);
            Controls.Add(txtKategorijaTarife);
            Controls.Add(txtStatus);
            Controls.Add(txtGrad);
            Controls.Add(txtAdresa);
            Controls.Add(txtTelefon);
            Controls.Add(txtEmail);
            Controls.Add(cmbTip);
            Name = "PotrosacIzmeniForma";
            Text = "PotrosacIzmeniForma";
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
        private TextBox txtKategorijaTarife;
        private Button btnSacuvaj;
    }
}