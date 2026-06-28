using app.Entities;
using app.Entities.Enums;
using app.Mappings.Types;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class PametnoBrojiloMap : ClassMap<PametnoBrojilo>
    {
        public PametnoBrojiloMap()
        {
            Table("PAMETNO");
            Not.LazyLoad();

            Id(x => x.SerijskiBroj)
                .Column("SERIJSKI_BROJ")
                .Length(50)
                .GeneratedBy.Foreign("Brojilo");

            HasOne(x => x.Brojilo)
                .Constrained();

            Map(x => x.Protokol)
                .Column("PROTOKOL")
                .CustomType<DdlEnumStringType<PametnoBrojiloProtokol>>()
                .Length(50)
                .Nullable();

            Map(x => x.Frekvencija)
                .Column("FREKVENCIJA")
                .Precision(10)
                .Scale(2)
                .Nullable();

            Map(x => x.IsDaljinskoIskljucenje)
                .Column("IS_DALJINSKO_ISKLJUCENJE")
                .CustomType<DdlEnumStringType<DaNe>>()
                .Length(1)
                .Not.Nullable();

            Map(x => x.NivoBaterije)
                .Column("NIVO_BATERIJE")
                .Precision(5)
                .Scale(2)
                .Nullable();
        }
    }
}
