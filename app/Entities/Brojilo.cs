using System;
using System.Collections.Generic;
using app.Entities.Enums;

namespace app.Entities
{
    public class Brojilo
    {
        public Brojilo()
        {
            TipoviBrojila = new List<TipBrojila>();
            DatumiZamene = new List<DateTime>();
            Potrosaci = new List<Potrosac>();
            Merenja = new List<Merenje>();
            Kvarovi = new List<Kvar>();
        }

        public virtual string SerijskiBroj { get; set; }

        public virtual DateTime DatumInstalacije { get; set; }

        public virtual string Status { get; set; }

        public virtual string Lokacija { get; set; }

        public virtual decimal? KoeficijentMnozenja { get; set; }

        public virtual string Komentar { get; set; }

        public virtual IList<TipBrojila> TipoviBrojila { get; protected set; }

        public virtual IList<DateTime> DatumiZamene { get; protected set; }

        public virtual IList<Potrosac> Potrosaci { get; protected set; }

        public virtual IList<Merenje> Merenja { get; protected set; }

        public virtual IList<Kvar> Kvarovi { get; protected set; }

        public virtual TrofaznoBrojilo Trofazno { get; set; }

        public virtual MehanickoBrojilo Mehanicko { get; set; }

        public virtual PametnoBrojilo Pametno { get; set; }

        public virtual RasvetnoBrojilo Rasvetno { get; set; }
    }
}
