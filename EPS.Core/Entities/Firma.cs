namespace app.Entities
{
    public class Firma
    {
        public Firma()
        {
        }

        public virtual long Id { get; protected set; }

        public virtual Potrosac Potrosac { get; set; }

        public virtual string Naziv { get; set; }

        public virtual string Pib { get; set; }
    }
}
