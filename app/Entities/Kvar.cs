using System;
using app.Entities.Enums;

namespace app.Entities
{
    public class Kvar
    {
        public Kvar()
        {
        }

        public virtual long Id { get; protected set; }

        public virtual DateTime DatumPrijave { get; set; }

        public virtual Brojilo Brojilo { get; set; }

        public virtual Potrosac Potrosac { get; set; }

        public virtual string TipKvara { get; set; }

        public virtual string OpisProblema { get; set; }

        public virtual KvarStatus Status { get; set; }

        public virtual DateTime? DatumOtklanjanja { get; set; }

        public virtual KvarPrioritet? Prioritet { get; set; }

        public virtual string Komentar { get; set; }

        public virtual string NadlezniTim { get; set; }
    }
}
