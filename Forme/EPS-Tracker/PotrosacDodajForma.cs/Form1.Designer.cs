namespace PotrosacDodajForma.cs
{
    partial class cmbTip
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            comboBox1 = new ComboBox();
            txtEmail = new TextBox();
            txtTelefon = new TextBox();
            txtAdresa = new TextBox();
            txtGrad = new TextBox();
            txtStatus = new TextBox();
            txtKategorijaTarife = new TextBox();
            btnSacuvaj = new Button();
            SuspendLayout();
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.Simple;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(12, 12);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 150);
            comboBox1.TabIndex = 0;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(155, 14);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(100, 23);
            txtEmail.TabIndex = 1;
            // 
            // txtTelefon
            // 
            txtTelefon.Location = new Point(155, 48);
            txtTelefon.Name = "txtTelefon";
            txtTelefon.Size = new Size(100, 23);
            txtTelefon.TabIndex = 2;
            // 
            // txtAdresa
            // 
            txtAdresa.Location = new Point(155, 85);
            txtAdresa.Name = "txtAdresa";
            txtAdresa.Size = new Size(100, 23);
            txtAdresa.TabIndex = 3;
            // 
            // txtGrad
            // 
            txtGrad.Location = new Point(156, 125);
            txtGrad.Name = "txtGrad";
            txtGrad.Size = new Size(100, 23);
            txtGrad.TabIndex = 4;
            // 
            // txtStatus
            // 
            txtStatus.Location = new Point(157, 165);
            txtStatus.Name = "txtStatus";
            txtStatus.Size = new Size(100, 23);
            txtStatus.TabIndex = 5;
            // 
            // txtKategorijaTarife
            // 
            txtKategorijaTarife.Location = new Point(156, 199);
            txtKategorijaTarife.Name = "txtKategorijaTarife";
            txtKategorijaTarife.Size = new Size(100, 23);
            txtKategorijaTarife.TabIndex = 6;
            // 
            // btnSacuvaj
            // 
            btnSacuvaj.Location = new Point(96, 251);
            btnSacuvaj.Name = "btnSacuvaj";
            btnSacuvaj.Size = new Size(75, 23);
            btnSacuvaj.TabIndex = 7;
            btnSacuvaj.Text = "button1";
            btnSacuvaj.UseVisualStyleBackColor = true;
            btnSacuvaj.Click += btnSacuvaj_Click;
            // 
            // cmbTip
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSacuvaj);
            Controls.Add(txtKategorijaTarife);
            Controls.Add(txtStatus);
            Controls.Add(txtGrad);
            Controls.Add(txtAdresa);
            Controls.Add(txtTelefon);
            Controls.Add(txtEmail);
            Controls.Add(comboBox1);
            Name = "cmbTip";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBox1;
        private TextBox txtEmail;
        private TextBox txtTelefon;
        private TextBox txtAdresa;
        private TextBox txtGrad;
        private TextBox txtStatus;
        private TextBox txtKategorijaTarife;
        private Button btnSacuvaj;
    }
}
