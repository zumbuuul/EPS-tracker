namespace app.Entities
{
    public class Domacinstvo
    {
        public Domacinstvo()
        {
        }

        public virtual long Id { get; protected set; }

        public virtual Potrosac Potrosac { get; set; }

        public virtual string Jmbg { get; set; }

        public virtual string Ime { get; set; }

        public virtual string Prezime { get; set; }
    }
}
