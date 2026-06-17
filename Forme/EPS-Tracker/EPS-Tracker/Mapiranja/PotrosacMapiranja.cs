using EPS_Tracker.Entiteti;
using FluentNHibernate.Mapping;

namespace EPS_Tracker.Mapiranja;

public class PotrosacMapiranja : ClassMap<Potrosac>
{
    public PotrosacMapiranja()
    {
        Table("POTROSAC");
        Not.LazyLoad();

        Id(x => x.Id)
            .Column("ID")
            .GeneratedBy.Identity();

        Map(x => x.Tip)
            .Column("TIP")
            .Not.Nullable();

        Map(x => x.Email)
            .Column("EMAIL")
            .Nullable();

        Map(x => x.Telefon)
            .Column("TELEFON")
            .Nullable();

        Map(x => x.Adresa)
            .Column("ADRESA")
            .Not.Nullable();

        Map(x => x.Grad)
            .Column("GRAD")
            .Not.Nullable();

        Map(x => x.Komentar)
            .Column("KOMENTAR")
            .Nullable();

        Map(x => x.Status)
            .Column("STATUS")
            .Not.Nullable();

        Map(x => x.KategorijaTarife)
            .Column("KATEGORIJA_TARIFE")
            .Not.Nullable();

        HasManyToMany(x => x.Brojila)
        .Table("POTROSAC_BROJILO_VEZA")
        .ParentKeyColumn("POTROSAC_ID")
        .ChildKeyColumn("SERIJSKI_BROJ")
        .Cascade.None()
        .AsBag();
    }

}