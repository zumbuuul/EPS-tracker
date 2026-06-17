namespace EPS_Tracker
{
    partial class GlavnaForma
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
            dataGridView1 = new DataGridView();
            button1 = new Button();
            button2 = new Button();
            btnDodajPotrosaca = new Button();
            btnObrisiPotrosaca = new Button();
            btnIzmeniPotrosaca = new Button();
            btnDodajBrojilo = new Button();
            btnObrisiBrojilo = new Button();
            btnIzmeniBrojilo = new Button();
            btnBrojilaPotrosaca = new Button();
            label1 = new Label();
            btnPoveziBrojilo = new Button();
            label2 = new Label();
            btnRaskiniVezu = new Button();
            btnMerenjaBrojila = new Button();
            label3 = new Label();
            oracleCommandBuilder1 = new Oracle.ManagedDataAccess.Client.OracleCommandBuilder();
            btnDodajMerenje = new Button();
            btnObrisiMerenje = new Button();
            label4 = new Label();
            btnIzmeniMerenje = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.BackgroundColor = SystemColors.Control;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 12);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(744, 339);
            dataGridView1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(12, 386);
            button1.Name = "button1";
            button1.Size = new Size(369, 23);
            button1.TabIndex = 1;
            button1.Text = "Prikazi potrosace";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(387, 386);
            button2.Name = "button2";
            button2.Size = new Size(369, 23);
            button2.TabIndex = 2;
            button2.Text = "Prikazi brojila";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // btnDodajPotrosaca
            // 
            btnDodajPotrosaca.Location = new Point(12, 415);
            btnDodajPotrosaca.Name = "btnDodajPotrosaca";
            btnDodajPotrosaca.Size = new Size(370, 23);
            btnDodajPotrosaca.TabIndex = 3;
            btnDodajPotrosaca.Text = "Dodaj potrosaca";
            btnDodajPotrosaca.UseVisualStyleBackColor = true;
            btnDodajPotrosaca.Click += btnDodajPotrosaca_Click;
            // 
            // btnObrisiPotrosaca
            // 
            btnObrisiPotrosaca.Location = new Point(13, 444);
            btnObrisiPotrosaca.Name = "btnObrisiPotrosaca";
            btnObrisiPotrosaca.Size = new Size(369, 23);
            btnObrisiPotrosaca.TabIndex = 4;
            btnObrisiPotrosaca.Text = "Obrisi potrosaca";
            btnObrisiPotrosaca.UseVisualStyleBackColor = true;
            btnObrisiPotrosaca.Click += btnObrisiPotrosaca_Click;
            // 
            // btnIzmeniPotrosaca
            // 
            btnIzmeniPotrosaca.Location = new Point(13, 473);
            btnIzmeniPotrosaca.Name = "btnIzmeniPotrosaca";
            btnIzmeniPotrosaca.Size = new Size(369, 23);
            btnIzmeniPotrosaca.TabIndex = 5;
            btnIzmeniPotrosaca.Text = "Izmeni potrošača";
            btnIzmeniPotrosaca.UseVisualStyleBackColor = true;
            btnIzmeniPotrosaca.Click += btnIzmeniPotrosaca_Click;
            // 
            // btnDodajBrojilo
            // 
            btnDodajBrojilo.Location = new Point(387, 415);
            btnDodajBrojilo.Name = "btnDodajBrojilo";
            btnDodajBrojilo.Size = new Size(369, 23);
            btnDodajBrojilo.TabIndex = 6;
            btnDodajBrojilo.Text = "Dodaj brojilo";
            btnDodajBrojilo.UseVisualStyleBackColor = true;
            btnDodajBrojilo.Click += btnDodajBrojilo_Click;
            // 
            // btnObrisiBrojilo
            // 
            btnObrisiBrojilo.Location = new Point(387, 444);
            btnObrisiBrojilo.Name = "btnObrisiBrojilo";
            btnObrisiBrojilo.Size = new Size(369, 23);
            btnObrisiBrojilo.TabIndex = 7;
            btnObrisiBrojilo.Text = "Obrisi brojilo";
            btnObrisiBrojilo.UseVisualStyleBackColor = true;
            btnObrisiBrojilo.Click += btnObrisiBrojilo_Click;
            // 
            // btnIzmeniBrojilo
            // 
            btnIzmeniBrojilo.Location = new Point(387, 473);
            btnIzmeniBrojilo.Name = "btnIzmeniBrojilo";
            btnIzmeniBrojilo.Size = new Size(369, 23);
            btnIzmeniBrojilo.TabIndex = 8;
            btnIzmeniBrojilo.Text = "Izmeni brojilo";
            btnIzmeniBrojilo.UseVisualStyleBackColor = true;
            btnIzmeniBrojilo.Click += btnIzmeniBrojilo_Click;
            // 
            // btnBrojilaPotrosaca
            // 
            btnBrojilaPotrosaca.Location = new Point(762, 41);
            btnBrojilaPotrosaca.Name = "btnBrojilaPotrosaca";
            btnBrojilaPotrosaca.Size = new Size(209, 23);
            btnBrojilaPotrosaca.TabIndex = 9;
            btnBrojilaPotrosaca.Text = "Brojila Potrosaca";
            btnBrojilaPotrosaca.UseVisualStyleBackColor = true;
            btnBrojilaPotrosaca.Click += btnBrojilaPotrosaca_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(771, 16);
            label1.Name = "label1";
            label1.Size = new Size(191, 15);
            label1.TabIndex = 10;
            label1.Text = "Prvo odabrati potrosaca pa kliknuti";
            label1.Click += label1_Click;
            // 
            // btnPoveziBrojilo
            // 
            btnPoveziBrojilo.Location = new Point(762, 70);
            btnPoveziBrojilo.Name = "btnPoveziBrojilo";
            btnPoveziBrojilo.Size = new Size(209, 23);
            btnPoveziBrojilo.TabIndex = 11;
            btnPoveziBrojilo.Text = "Povezi brojilo";
            btnPoveziBrojilo.UseVisualStyleBackColor = true;
            btnPoveziBrojilo.Click += btnPoveziBrojilo_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(545, 412);
            label2.Name = "label2";
            label2.Size = new Size(0, 15);
            label2.TabIndex = 12;
            // 
            // btnRaskiniVezu
            // 
            btnRaskiniVezu.Location = new Point(762, 99);
            btnRaskiniVezu.Name = "btnRaskiniVezu";
            btnRaskiniVezu.Size = new Size(209, 23);
            btnRaskiniVezu.TabIndex = 13;
            btnRaskiniVezu.Text = "Raskini vezu Potrosac-Brojlio";
            btnRaskiniVezu.UseVisualStyleBackColor = true;
            btnRaskiniVezu.Click += btnRaskiniVezu_Click;
            // 
            // btnMerenjaBrojila
            // 
            btnMerenjaBrojila.Location = new Point(762, 178);
            btnMerenjaBrojila.Name = "btnMerenjaBrojila";
            btnMerenjaBrojila.Size = new Size(209, 23);
            btnMerenjaBrojila.TabIndex = 14;
            btnMerenjaBrojila.Text = "Merenja brojila";
            btnMerenjaBrojila.UseVisualStyleBackColor = true;
            btnMerenjaBrojila.Click += btnMerenjaBrojila_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(771, 157);
            label3.Name = "label3";
            label3.Size = new Size(203, 15);
            label3.TabIndex = 15;
            label3.Text = "Prvo odabrati brojilo pa onda kliknuti";
            // 
            // btnDodajMerenje
            // 
            btnDodajMerenje.Location = new Point(762, 207);
            btnDodajMerenje.Name = "btnDodajMerenje";
            btnDodajMerenje.Size = new Size(209, 23);
            btnDodajMerenje.TabIndex = 16;
            btnDodajMerenje.Text = "Dodaj merenje";
            btnDodajMerenje.UseVisualStyleBackColor = true;
            btnDodajMerenje.Click += btnDodajMerenje_Click;
            // 
            // btnObrisiMerenje
            // 
            btnObrisiMerenje.Location = new Point(763, 271);
            btnObrisiMerenje.Name = "btnObrisiMerenje";
            btnObrisiMerenje.Size = new Size(208, 23);
            btnObrisiMerenje.TabIndex = 17;
            btnObrisiMerenje.Text = "Obrisi merenje";
            btnObrisiMerenje.UseVisualStyleBackColor = true;
            btnObrisiMerenje.Click += btnObrisiMerenje_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(763, 251);
            label4.Name = "label4";
            label4.Size = new Size(212, 15);
            label4.TabIndex = 18;
            label4.Text = "Prvo odabrati merenje pa onda kliknuti";
            // 
            // btnIzmeniMerenje
            // 
            btnIzmeniMerenje.Location = new Point(762, 300);
            btnIzmeniMerenje.Name = "btnIzmeniMerenje";
            btnIzmeniMerenje.Size = new Size(209, 23);
            btnIzmeniMerenje.TabIndex = 19;
            btnIzmeniMerenje.Text = "Izmeni merenje";
            btnIzmeniMerenje.UseVisualStyleBackColor = true;
            btnIzmeniMerenje.Click += btnIzmeniMerenje_Click;
            // 
            // GlavnaForma
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(983, 650);
            Controls.Add(btnIzmeniMerenje);
            Controls.Add(label4);
            Controls.Add(btnObrisiMerenje);
            Controls.Add(btnDodajMerenje);
            Controls.Add(label3);
            Controls.Add(btnMerenjaBrojila);
            Controls.Add(btnRaskiniVezu);
            Controls.Add(label2);
            Controls.Add(btnPoveziBrojilo);
            Controls.Add(label1);
            Controls.Add(btnBrojilaPotrosaca);
            Controls.Add(btnIzmeniBrojilo);
            Controls.Add(btnObrisiBrojilo);
            Controls.Add(btnDodajBrojilo);
            Controls.Add(btnIzmeniPotrosaca);
            Controls.Add(btnObrisiPotrosaca);
            Controls.Add(btnDodajPotrosaca);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Name = "GlavnaForma";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button button1;
        private Button button2;
        private Button btnDodajPotrosaca;
        private Button btnObrisiPotrosaca;
        private Button btnIzmeniPotrosaca;
        private Button btnDodajBrojilo;
        private Button btnObrisiBrojilo;
        private Button btnIzmeniBrojilo;
        private Button btnBrojilaPotrosaca;
        private Label label1;
        private Button btnPoveziBrojilo;
        private Label label2;
        private Button btnRaskiniVezu;
        private Button btnMerenjaBrojila;
        private Label label3;
        private Oracle.ManagedDataAccess.Client.OracleCommandBuilder oracleCommandBuilder1;
        private Button btnDodajMerenje;
        private Button btnObrisiMerenje;
        private Label label4;
        private Button btnIzmeniMerenje;
    }
}
