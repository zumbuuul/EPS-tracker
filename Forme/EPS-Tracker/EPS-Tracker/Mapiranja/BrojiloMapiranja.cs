using System;
using System.Collections.Generic;
using System.Text;
using EPS_Tracker.Entiteti;
using FluentNHibernate.Mapping;

namespace EPS_Tracker.Mapiranja;

public class BrojiloMapiranja : ClassMap<Brojilo>
{
    public BrojiloMapiranja()
    {
        Table("BROJILO");
        Not.LazyLoad();

        Id(x => x.SerijskiBroj)
            .Column("SERIJSKI_BROJ")
            .GeneratedBy.Assigned();

        Map(x => x.DatumInstalacije)
            .Column("DATUM_INSTALACIJE")
            .Not.Nullable();

        Map(x => x.Status)
            .Column("STATUS")
            .Not.Nullable();

        Map(x => x.Lokacija)
            .Column("LOKACIJA")
            .Nullable();

        Map(x => x.KoeficijentMnozenja)
            .Column("KOEFICIJENT_MNOZENJA")
            .Nullable();

        Map(x => x.Komentar)
            .Column("KOMENTAR")
            .Nullable();

        HasManyToMany(x => x.Potrosaci)
        .Table("POTROSAC_BROJILO_VEZA")
        .ParentKeyColumn("SERIJSKI_BROJ")
        .ChildKeyColumn("POTROSAC_ID")
        .Cascade.None()
        .Inverse()
        .AsBag();

        HasMany(x => x.Merenja)
        .KeyColumn("SERIJSKI_BROJ")
        .Inverse()
        .Cascade.None()
        .AsBag();
    }
}
