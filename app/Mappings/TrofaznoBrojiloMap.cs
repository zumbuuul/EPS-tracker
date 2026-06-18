using app.Entities;
using app.Entities.Enums;
using app.Mappings.Types;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class TrofaznoBrojiloMap : ClassMap<TrofaznoBrojilo>
    {
        public TrofaznoBrojiloMap()
        {
            Table("TROFAZNO");
            Not.LazyLoad();

            Id(x => x.SerijskiBroj)
                .Column("SERIJSKI_BROJ")
                .Length(50)
                .GeneratedBy.Foreign("Brojilo");

            HasOne(x => x.Brojilo)
                .Constrained();

            Map(x => x.MaxSnaga)
                .Column("MAX_SNAGA")
                .Precision(10)
                .Scale(4)
                .Nullable();

            Map(x => x.MogucnostMerenjaPoZonama)
                .Column("MOGUCNOST_MERENJA_PO_ZONAMA")
                .CustomType<DdlEnumStringType<DaNe>>()
                .Length(1)
                .Not.Nullable();

            Map(x => x.UgovorenaSnaga)
                .Column("UGOVORENA_SNAGA")
                .Precision(10)
                .Scale(4)
                .Nullable();
        }
    }
}
