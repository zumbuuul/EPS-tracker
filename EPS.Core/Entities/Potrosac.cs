using System.Collections.Generic;
using System.Linq;
using app.Entities.Enums;

namespace app.Entities
{
    public class Potrosac
    {
        public Potrosac()
        {
            VezeBrojila = new List<PotrosacBrojiloVeza>();
            Racuni = new List<Racun>();
            Kvarovi = new List<Kvar>();
        }

        public virtual long Id { get; protected set; }

        public virtual PotrosacTip Tip { get; set; }

        public virtual string Email { get; set; }

        public virtual string Telefon { get; set; }

        public virtual string Adresa { get; set; }

        public virtual string Grad { get; set; }

        public virtual string Komentar { get; set; }

        public virtual string Status { get; set; }

        public virtual string KategorijaTarife { get; set; }

        public virtual Domacinstvo Domacinstvo { get; set; }

        public virtual Firma Firma { get; set; }

        public virtual IList<PotrosacBrojiloVeza> VezeBrojila { get; protected set; }

        public virtual IList<Brojilo> Brojila
        {
            get
            {
                return VezeBrojila
                    .Where(x => x.Brojilo != null && !x.DatumDo.HasValue)
                    .Select(x => x.Brojilo)
                    .ToList();
            }
        }

        public virtual IList<Racun> Racuni { get; protected set; }

        public virtual IList<Kvar> Kvarovi { get; protected set; }
    }
}
