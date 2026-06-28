using app.Entities;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class RasvetnoBrojiloMap : ClassMap<RasvetnoBrojilo>
    {
        public RasvetnoBrojiloMap()
        {
            Table("RASVETNO");
            Not.LazyLoad();

            Id(x => x.SerijskiBroj)
                .Column("SERIJSKI_BROJ")
                .Length(50)
                .GeneratedBy.Foreign("Brojilo");

            HasOne(x => x.Brojilo)
                .Constrained();

            Map(x => x.VremeUkljucenja)
                .Column("VREME_UKLJUCENJA")
                .Length(5)
                .Nullable();

            Map(x => x.VremeIskljucenja)
                .Column("VREME_ISKLJUCENJA")
                .Length(5)
                .Nullable();

            Map(x => x.StatusSenzora)
                .Column("STATUS_SENZORA")
                .Length(50)
                .Nullable();

            Map(x => x.RadnoVreme)
                .Column("RADNO_VREME")
                .Precision(6)
                .Scale(2)
                .Nullable();
        }
    }
}
