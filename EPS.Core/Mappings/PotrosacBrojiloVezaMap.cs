using app.Entities;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class PotrosacBrojiloVezaMap : ClassMap<PotrosacBrojiloVeza>
    {
        public PotrosacBrojiloVezaMap()
        {
            Table("POTROSAC_BROJILO_VEZA");
            Not.LazyLoad();

            Id(x => x.Id)
                .Column("ID")
                .GeneratedBy.TriggerIdentity();

            References(x => x.Potrosac)
                .Column("POTROSAC_ID")
                .Not.Nullable();

            References(x => x.Brojilo)
                .Column("SERIJSKI_BROJ")
                .Not.Nullable();

            Map(x => x.DatumOd)
                .Column("DATUM_OD")
                .Not.Nullable();

            Map(x => x.DatumDo)
                .Column("DATUM_DO")
                .Nullable();
        }
    }
}
