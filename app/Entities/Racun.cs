using System;

namespace app.Entities
{
    public class Racun
    {
        public Racun()
        {
        }

        public virtual string BrojRacuna { get; set; }

        public virtual DateTime DatumIzdavanja { get; set; }

        public virtual DateTime RokPlacanja { get; set; }

        public virtual string Komentar { get; set; }

        public virtual string NacinPlacanja { get; set; }

        public virtual DateTime PeriodPotrosnjeOd { get; set; }

        public virtual DateTime PeriodPotrosnjeDo { get; set; }

        public virtual decimal IznosBezPdv { get; set; }

        public virtual decimal Pdv { get; set; }

        public virtual string Status { get; set; }

        public virtual Merenje Merenje { get; set; }

        public virtual Potrosac Potrosac { get; set; }
    }
}
