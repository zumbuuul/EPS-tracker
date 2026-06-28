using app.Entities.Enums;

namespace app.Entities
{
    public class TrofaznoBrojilo
    {
        public TrofaznoBrojilo()
        {
        }

        public virtual string SerijskiBroj { get; set; }

        public virtual Brojilo Brojilo { get; set; }

        public virtual decimal? MaxSnaga { get; set; }

        public virtual DaNe MogucnostMerenjaPoZonama { get; set; }

        public virtual decimal? UgovorenaSnaga { get; set; }
    }
}
