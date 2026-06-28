using System;

namespace app.Entities
{
    public class Stanje
    {
        public Stanje()
        {
        }

        public virtual long Id { get; protected set; }

        public virtual DateTime DatumIVreme { get; set; }

        public virtual string Lokacija { get; set; }

        public virtual decimal NaponskiNivo { get; set; }

        public virtual string Status { get; set; }

        public virtual string Transformator { get; set; }

        public virtual string Komentar { get; set; }

        public virtual decimal UkupnaPotrosnja { get; set; }

        public virtual decimal Gubitak { get; set; }

        public virtual decimal DistribuiranaEnergija { get; set; }

        public virtual decimal ProizvedenaEnergija { get; set; }
    }
}
