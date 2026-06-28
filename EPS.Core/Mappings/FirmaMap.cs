using app.Entities;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class FirmaMap : ClassMap<Firma>
    {
        public FirmaMap()
        {
            Table("FIRMA");
            Not.LazyLoad();

            Id(x => x.Id)
                .Column("ID")
                .GeneratedBy.Foreign("Potrosac");

            HasOne(x => x.Potrosac)
                .Constrained();

            Map(x => x.Naziv)
                .Column("NAZIV")
                .Length(200)
                .Not.Nullable();

            Map(x => x.Pib)
                .Column("PIB")
                .Length(9)
                .Unique()
                .Not.Nullable();
        }
    }
}
