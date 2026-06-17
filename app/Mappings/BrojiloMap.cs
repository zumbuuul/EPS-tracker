using app.Entities;
using app.Entities.Enums;
using app.Mappings.Types;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class BrojiloMap : ClassMap<Brojilo>
    {
        public BrojiloMap()
        {
            Table("BROJILO");

            Id(x => x.SerijskiBroj)
                .Column("SERIJSKI_BROJ")
                .Length(50)
                .GeneratedBy.Assigned();

            Map(x => x.DatumInstalacije)
                .Column("DATUM_INSTALACIJE")
                .Not.Nullable();

            Map(x => x.Status)
                .Column("STATUS")
                .Length(50)
                .Not.Nullable();

            Map(x => x.Lokacija)
                .Column("LOKACIJA")
                .Length(200)
                .Nullable();

            Map(x => x.KoeficijentMnozenja)
                .Column("KOEFICIJENT_MNOZENJA")
                .Precision(10)
                .Scale(4)
                .Nullable();

            Map(x => x.Komentar)
                .Column("KOMENTAR")
                .Length(500)
                .Nullable();

            HasMany(x => x.TipoviBrojila)
                .Table("TIP_BROJILA")
                .KeyColumn("SERIJSKI_BROJ")
                .Element("TIP_BROJILA", element => element.Type<DdlEnumStringType<TipBrojila>>())
                .AsBag()
                .Cascade.AllDeleteOrphan();

            HasMany(x => x.DatumiZamene)
                .Table("DATUM_ZAMENE")
                .KeyColumn("SERIJSKI_BROJ")
                .Element("DATUM_ZAMENE")
                .AsBag()
                .Cascade.AllDeleteOrphan();

            HasManyToMany(x => x.Potrosaci)
                .Table("POTROSAC_BROJILO_VEZA")
                .ParentKeyColumn("SERIJSKI_BROJ")
                .ChildKeyColumn("POTROSAC_ID")
                .Inverse()
                .AsBag();

            HasMany(x => x.Merenja)
                .KeyColumn("SERIJSKI_BROJ")
                .Inverse()
                .AsBag();

            HasMany(x => x.Kvarovi)
                .KeyColumn("SERIJSKI_BROJ")
                .Inverse()
                .AsBag();

            HasOne(x => x.Trofazno)
                .Cascade.All();

            HasOne(x => x.Mehanicko)
                .Cascade.All();

            HasOne(x => x.Pametno)
                .Cascade.All();

            HasOne(x => x.Rasvetno)
                .Cascade.All();
        }
    }
}
