using app.Entities;
using app.Entities.Enums;
using app.Mappings.Types;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class MerenjeMap : ClassMap<Merenje>
    {
        public MerenjeMap()
        {
            Table("MERENJE");

            Id(x => x.Id)
                .Column("ID")
                .GeneratedBy.Identity();

            References(x => x.Brojilo)
                .Column("SERIJSKI_BROJ")
                .Not.Nullable();

            Map(x => x.DatumVremeMerenja)
                .Column("DATUM_VREME_MERENJA")
                .Not.Nullable();

            Map(x => x.PotrosnjaAktivna)
                .Column("POTROSNJA_AKTIVNA")
                .Precision(15)
                .Scale(4)
                .Nullable();

            Map(x => x.PotrosnjaReaktivna)
                .Column("POTROSNJA_REAKTIVNA")
                .Precision(15)
                .Scale(4)
                .Nullable();

            Map(x => x.Snaga)
                .Column("SNAGA")
                .Precision(15)
                .Scale(4)
                .Nullable();

            Map(x => x.Napon)
                .Column("NAPON")
                .Precision(10)
                .Scale(4)
                .Nullable();

            Map(x => x.Struja)
                .Column("STRUJA")
                .Precision(10)
                .Scale(4)
                .Nullable();

            Map(x => x.TipMerenja)
                .Column("TIP_MERENJA")
                .Length(50)
                .Nullable();

            Map(x => x.TipIzvora)
                .Column("TIP_IZVORA")
                .CustomType<DdlEnumStringType<TipIzvoraMerenja>>()
                .Length(50)
                .Nullable();

            Map(x => x.IsValidirano)
                .Column("IS_VALIDIRANO")
                .CustomType<DdlEnumStringType<DaNe>>()
                .Length(1)
                .Not.Nullable();

            Map(x => x.Komentar)
                .Column("KOMENTAR")
                .Length(500)
                .Nullable();
        }
    }
}
