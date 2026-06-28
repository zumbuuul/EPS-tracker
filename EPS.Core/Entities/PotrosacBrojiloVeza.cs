using System;

namespace app.Entities
{
    public class PotrosacBrojiloVeza
    {
        public virtual long Id { get; protected set; }

        public virtual Potrosac Potrosac { get; set; }

        public virtual Brojilo Brojilo { get; set; }

        public virtual DateTime DatumOd { get; set; }

        public virtual DateTime? DatumDo { get; set; }
    }
}
