using System;
using app.Entities.Enums;

namespace app.Entities
{
    public class Merenje
    {
        public Merenje()
        {
        }

        public virtual long Id { get; protected set; }

        public virtual DateTime DatumVremeMerenja { get; set; }

        public virtual Brojilo Brojilo { get; set; }

        public virtual decimal? PotrosnjaAktivna { get; set; }

        public virtual decimal? PotrosnjaReaktivna { get; set; }

        public virtual decimal? Snaga { get; set; }

        public virtual decimal? Napon { get; set; }

        public virtual decimal? Struja { get; set; }

        public virtual string TipMerenja { get; set; }

        public virtual TipIzvoraMerenja? TipIzvora { get; set; }

        public virtual DaNe IsValidirano { get; set; }

        public virtual string Komentar { get; set; }
    }
}
