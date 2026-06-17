namespace EPS_Tracker
{
    partial class PoveziPotrosacaBrojiloForma
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
            txtPotrosacId = new TextBox();
            txtSerijskiBroj = new TextBox();
            btnPovezi = new Button();
            SuspendLayout();
            // 
            // txtPotrosacId
            // 
            txtPotrosacId.Location = new Point(12, 12);
            txtPotrosacId.Name = "txtPotrosacId";
            txtPotrosacId.ReadOnly = true;
            txtPotrosacId.Size = new Size(168, 23);
            txtPotrosacId.TabIndex = 0;
            // 
            // txtSerijskiBroj
            // 
            txtSerijskiBroj.Location = new Point(12, 41);
            txtSerijskiBroj.Name = "txtSerijskiBroj";
            txtSerijskiBroj.Size = new Size(168, 23);
            txtSerijskiBroj.TabIndex = 1;
            // 
            // btnPovezi
            // 
            btnPovezi.Location = new Point(12, 70);
            btnPovezi.Name = "btnPovezi";
            btnPovezi.Size = new Size(168, 23);
            btnPovezi.TabIndex = 2;
            btnPovezi.Text = "Povezi";
            btnPovezi.UseVisualStyleBackColor = true;
            btnPovezi.Click += btnPovezi_Click;
            // 
            // PoveziPotrosacaBrojiloForma
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(214, 114);
            Controls.Add(btnPovezi);
            Controls.Add(txtSerijskiBroj);
            Controls.Add(txtPotrosacId);
            Name = "PoveziPotrosacaBrojiloForma";
            Text = "PoveziPotrosacaBrojiloForma";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtPotrosacId;
        private TextBox txtSerijskiBroj;
        private Button btnPovezi;
    }
}