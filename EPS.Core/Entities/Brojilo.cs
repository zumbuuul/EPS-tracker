using System;
using System.Collections.Generic;
using System.Linq;
using app.Entities.Enums;

namespace app.Entities
{
    public class Brojilo
    {
        public Brojilo()
        {
            TipoviBrojila = new List<TipBrojila>();
            DatumiZamene = new List<DateTime>();
            VezePotrosaca = new List<PotrosacBrojiloVeza>();
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

        public virtual IList<PotrosacBrojiloVeza> VezePotrosaca { get; protected set; }

        public virtual IList<Potrosac> Potrosaci
        {
            get
            {
                return VezePotrosaca
                    .Where(x => x.Potrosac != null && !x.DatumDo.HasValue)
                    .Select(x => x.Potrosac)
                    .ToList();
            }
        }

        public virtual IList<Merenje> Merenja { get; protected set; }

        public virtual IList<Kvar> Kvarovi { get; protected set; }

        public virtual TrofaznoBrojilo Trofazno { get; set; }

        public virtual MehanickoBrojilo Mehanicko { get; set; }

        public virtual PametnoBrojilo Pametno { get; set; }

        public virtual RasvetnoBrojilo Rasvetno { get; set; }
    }
}
