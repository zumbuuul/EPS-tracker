using static EPS_Tracker.PotrosacPregled;

namespace EPS_Tracker
{
    public partial class GlavnaForma : Form
    {
        public GlavnaForma()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("Id", "ID");
            dataGridView1.Columns.Add("Tip", "Tip");
            dataGridView1.Columns.Add("Email", "Email");
            dataGridView1.Columns.Add("Telefon", "Telefon");
            dataGridView1.Columns.Add("Adresa", "Adresa");
            dataGridView1.Columns.Add("Grad", "Grad");
            dataGridView1.Columns.Add("Status", "Status");

            List<PotrosacPregled> potrosaci = DTOManager.VratiPotrosace();

            foreach (var p in potrosaci)
            {
                dataGridView1.Rows.Add(
                    p.Id,
                    p.Tip,
                    p.Email,
                    p.Telefon,
                    p.Adresa,
                    p.Grad,
                    p.Status
                );
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("SerijskiBroj", "Serijski broj");
            dataGridView1.Columns.Add("DatumInstalacije", "Datum instalacije");
            dataGridView1.Columns.Add("Status", "Status");
            dataGridView1.Columns.Add("Lokacija", "Lokacija");
            dataGridView1.Columns.Add("KoeficijentMnozenja", "Koeficijent množenja");

            List<BrojiloPregled> brojila = DTOManager.VratiBrojila();

            foreach (var b in brojila)
            {
                dataGridView1.Rows.Add(
                    b.SerijskiBroj,
                    b.DatumInstalacije.ToShortDateString(),
                    b.Status,
                    b.Lokacija,
                    b.KoeficijentMnozenja?.ToString() ?? ""
                );
            }
        }

        private void btnDodajPotrosaca_Click(object sender, EventArgs e)
        {
            PotrosacDodajForma forma = new PotrosacDodajForma();

            if (forma.ShowDialog() == DialogResult.OK)
            {
                button1_Click(sender, e); // ponovo učita potrošače
            }
        }

        private void btnObrisiPotrosaca_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi potrošača za brisanje.");
                return;
            }

            string? idText = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (!long.TryParse(idText, out long id))
            {
                MessageBox.Show("Ne mogu da pročitam ID potrošača.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Da li sigurno želiš da obrišeš potrošača?",
                "Potvrda brisanja",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
            {
                return;
            }

            bool uspesno = DTOManager.ObrisiPotrosaca(id);

            if (uspesno)
            {
                MessageBox.Show("Potrošač je obrisan.");
                button1_Click(sender, e);
            }
        }

        private void btnIzmeniPotrosaca_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi potrošača za izmenu.");
                return;
            }

            string? idText = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (!long.TryParse(idText, out long id))
            {
                MessageBox.Show("Ne mogu da pročitam ID potrošača.");
                return;
            }

            PotrosacBasic pb = DTOManager.VratiPotrosacaBasic(id);

            PotrosacIzmeniForma forma = new PotrosacIzmeniForma(pb);

            if (forma.ShowDialog() == DialogResult.OK)
            {
                button1_Click(sender, e);
            }
        }

        private void btnDodajBrojilo_Click(object sender, EventArgs e)
        {
            BrojiloDodajForma forma = new BrojiloDodajForma();

            if (forma.ShowDialog() == DialogResult.OK)
            {
                button2_Click(sender, e);
            }
        }

        private void btnObrisiBrojilo_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi brojilo za brisanje.");
                return;
            }

            string? serijskiBroj = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (string.IsNullOrWhiteSpace(serijskiBroj))
            {
                MessageBox.Show("Ne mogu da pročitam serijski broj.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Da li sigurno želiš da obrišeš brojilo?",
                "Potvrda brisanja",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
            {
                return;
            }

            bool uspesno = DTOManager.ObrisiBrojilo(serijskiBroj);

            if (uspesno)
            {
                MessageBox.Show("Brojilo je obrisano.");
                button2_Click(sender, e);
            }
        }

        private void btnIzmeniBrojilo_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi brojilo za izmenu.");
                return;
            }

            string? serijskiBroj = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (string.IsNullOrWhiteSpace(serijskiBroj))
            {
                MessageBox.Show("Ne mogu da pročitam serijski broj.");
                return;
            }

            BrojiloBasic bb = DTOManager.VratiBrojiloBasic(serijskiBroj);

            BrojiloIzmeniForma forma = new BrojiloIzmeniForma(bb);

            if (forma.ShowDialog() == DialogResult.OK)
            {
                button2_Click(sender, e);
            }
        }

        private void btnBrojilaPotrosaca_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi potrošača.");
                return;
            }

            string? idText = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (!long.TryParse(idText, out long potrosacId))
            {
                MessageBox.Show("Ne mogu da pročitam ID potrošača.");
                return;
            }

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("SerijskiBroj", "Serijski broj");
            dataGridView1.Columns.Add("DatumInstalacije", "Datum instalacije");
            dataGridView1.Columns.Add("Status", "Status");
            dataGridView1.Columns.Add("Lokacija", "Lokacija");
            dataGridView1.Columns.Add("KoeficijentMnozenja", "Koeficijent množenja");

            List<BrojiloPregled> brojila = DTOManager.VratiBrojilaZaPotrosaca(potrosacId);

            foreach (var b in brojila)
            {
                dataGridView1.Rows.Add(
                    b.SerijskiBroj,
                    b.DatumInstalacije.ToShortDateString(),
                    b.Status,
                    b.Lokacija,
                    b.KoeficijentMnozenja?.ToString() ?? ""
                );
            }
        }

        private void btnPoveziBrojilo_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi potrošača.");
                return;
            }

            string? idText = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (!long.TryParse(idText, out long potrosacId))
            {
                MessageBox.Show("Ne mogu da pročitam ID potrošača.");
                return;
            }

            PoveziPotrosacaBrojiloForma forma = new PoveziPotrosacaBrojiloForma(potrosacId);

            if (forma.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Veza je uspešno dodata.");
            }
        }

        private void btnRaskiniVezu_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi potrošača.");
                return;
            }

            string? idText = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (!long.TryParse(idText, out long potrosacId))
            {
                MessageBox.Show("Ne mogu da pročitam ID potrošača.");
                return;
            }

            string serijskiBroj = Microsoft.VisualBasic.Interaction.InputBox(
                "Unesi serijski broj brojila:",
                "Raskidanje veze",
                ""
            );

            if (string.IsNullOrWhiteSpace(serijskiBroj))
            {
                return;
            }

            bool uspesno = DTOManager.RaskiniVezuPotrosacBrojilo(potrosacId, serijskiBroj);

            if (uspesno)
            {
                MessageBox.Show("Veza je raskinuta.");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnMerenjaBrojila_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi brojilo.");
                return;
            }

            string? serijskiBroj = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (string.IsNullOrWhiteSpace(serijskiBroj))
            {
                MessageBox.Show("Ne mogu da pročitam serijski broj.");
                return;
            }

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("Id", "ID");
            dataGridView1.Columns.Add("SerijskiBroj", "Serijski broj");
            dataGridView1.Columns.Add("DatumVremeMerenja", "Datum merenja");
            dataGridView1.Columns.Add("PotrosnjaAktivna", "Aktivna potrošnja");
            dataGridView1.Columns.Add("Snaga", "Snaga");
            dataGridView1.Columns.Add("Napon", "Napon");
            dataGridView1.Columns.Add("IsValidirano", "Validirano");

            List<MerenjePregled> merenja = DTOManager.VratiMerenjaZaBrojilo(serijskiBroj);

            foreach (var m in merenja)
            {
                dataGridView1.Rows.Add(
                    m.Id,
                    m.SerijskiBroj,
                    m.DatumVremeMerenja.ToString("g"),
                    m.PotrosnjaAktivna?.ToString() ?? "",
                    m.Snaga?.ToString() ?? "",
                    m.Napon?.ToString() ?? "",
                    m.IsValidirano
                );
            }
        }

        private void btnDodajMerenje_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi brojilo.");
                return;
            }

            string? serijskiBroj = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (string.IsNullOrWhiteSpace(serijskiBroj))
            {
                MessageBox.Show("Ne mogu da pročitam serijski broj.");
                return;
            }

            MerenjeDodajForma forma = new MerenjeDodajForma(serijskiBroj);

            if (forma.ShowDialog() == DialogResult.OK)
            {
                btnMerenjaBrojila_Click(sender, e);
            }
        }

        private void btnObrisiMerenje_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi merenje za brisanje.");
                return;
            }

            string? idText = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (!long.TryParse(idText, out long id))
            {
                MessageBox.Show("Ne mogu da pročitam ID merenja.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Da li sigurno želiš da obrišeš merenje?",
                "Potvrda brisanja",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
            {
                return;
            }

            bool uspesno = DTOManager.ObrisiMerenje(id);

            if (uspesno)
            {
                MessageBox.Show("Merenje je obrisano.");
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
        }

        private void btnIzmeniMerenje_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Izaberi merenje za izmenu.");
                return;
            }

            string? idText = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();

            if (!long.TryParse(idText, out long id))
            {
                MessageBox.Show("Ne mogu da pročitam ID merenja.");
                return;
            }

            MerenjeBasic mb = DTOManager.VratiMerenjeBasic(id);

            MerenjeIzmeniForma forma = new MerenjeIzmeniForma(mb);

            if (forma.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                MessageBox.Show("Klikni opet 'Merenja brojila' da osvežiš prikaz.");
            }
        }
    }
}

