using app.Entities.Enums;

namespace app.Entities
{
    public class PametnoBrojilo
    {
        public PametnoBrojilo()
        {
        }

        public virtual string SerijskiBroj { get; set; }

        public virtual Brojilo Brojilo { get; set; }

        public virtual PametnoBrojiloProtokol? Protokol { get; set; }

        public virtual decimal? Frekvencija { get; set; }

        public virtual DaNe IsDaljinskoIskljucenje { get; set; }

        public virtual decimal? NivoBaterije { get; set; }
    }
}
