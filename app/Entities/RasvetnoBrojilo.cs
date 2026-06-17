namespace app.Entities
{
    public class RasvetnoBrojilo
    {
        public RasvetnoBrojilo()
        {
        }

        public virtual string SerijskiBroj { get; set; }

        public virtual Brojilo Brojilo { get; set; }

        public virtual string VremeUkljucenja { get; set; }

        public virtual string VremeIskljucenja { get; set; }

        public virtual string StatusSenzora { get; set; }

        public virtual decimal? RadnoVreme { get; set; }
    }
}
