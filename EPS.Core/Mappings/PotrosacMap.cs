using app.Entities;
using app.Entities.Enums;
using app.Mappings.Types;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class PotrosacMap : ClassMap<Potrosac>
    {
        public PotrosacMap()
        {
            Table("POTROSAC");
            Not.LazyLoad();

            Id(x => x.Id)
                .Column("ID")
                .GeneratedBy.TriggerIdentity();

            Map(x => x.Tip)
                .Column("TIP")
                .CustomType<DdlEnumStringType<PotrosacTip>>()
                .Length(50)
                .Not.Nullable();

            Map(x => x.Email)
                .Column("EMAIL")
                .Length(100)
                .Nullable();

            Map(x => x.Telefon)
                .Column("TELEFON")
                .Length(30)
                .Nullable();

            Map(x => x.Adresa)
                .Column("ADRESA")
                .Length(200)
                .Not.Nullable();

            Map(x => x.Grad)
                .Column("GRAD")
                .Length(100)
                .Not.Nullable();

            Map(x => x.Komentar)
                .Column("KOMENTAR")
                .Length(500)
                .Nullable();

            Map(x => x.Status)
                .Column("STATUS")
                .Length(50)
                .Not.Nullable();

            Map(x => x.KategorijaTarife)
                .Column("KATEGORIJA_TARIFE")
                .Length(50)
                .Not.Nullable();

            HasOne(x => x.Domacinstvo)
                .Cascade.All();

            HasOne(x => x.Firma)
                .Cascade.All();

            HasMany(x => x.VezeBrojila)
                .KeyColumn("POTROSAC_ID")
                .Inverse()
                .AsBag();

            HasMany(x => x.Racuni)
                .KeyColumn("POTROSAC_ID")
                .Inverse()
                .AsBag();

            HasMany(x => x.Kvarovi)
                .KeyColumn("POTROSAC_ID")
                .Inverse()
                .AsBag();
        }
    }
}
