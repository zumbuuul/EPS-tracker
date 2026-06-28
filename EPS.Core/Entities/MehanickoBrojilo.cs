using System;

namespace app.Entities
{
    public class MehanickoBrojilo
    {
        public MehanickoBrojilo()
        {
        }

        public virtual string SerijskiBroj { get; set; }

        public virtual Brojilo Brojilo { get; set; }

        public virtual DateTime? PoslednjaKalibracija { get; set; }

        public virtual decimal? MaxGreska { get; set; }

        public virtual string Preciznost { get; set; }
    }
}
