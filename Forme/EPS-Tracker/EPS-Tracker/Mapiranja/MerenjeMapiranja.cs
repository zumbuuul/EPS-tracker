using System;
using System.Collections.Generic;
using System.Text;

using EPS_Tracker.Entiteti;
using FluentNHibernate.Mapping;

namespace EPS_Tracker.Mapiranja;

public class MerenjeMapiranja : ClassMap<Merenje>
{
    public MerenjeMapiranja()
    {
        Table("MERENJE");
        Not.LazyLoad();

        Id(x => x.Id)
        .Column("ID")
        .GeneratedBy.TriggerIdentity();

        References(x => x.Brojilo)
            .Column("SERIJSKI_BROJ")
            .Not.Nullable();

        Map(x => x.DatumVremeMerenja)
            .Column("DATUM_VREME_MERENJA")
            .Not.Nullable();

        Map(x => x.PotrosnjaAktivna)
            .Column("POTROSNJA_AKTIVNA")
            .Nullable();

        Map(x => x.PotrosnjaReaktivna)
            .Column("POTROSNJA_REAKTIVNA")
            .Nullable();

        Map(x => x.Snaga)
            .Column("SNAGA")
            .Nullable();

        Map(x => x.Napon)
            .Column("NAPON")
            .Nullable();

        Map(x => x.Struja)
            .Column("STRUJA")
            .Nullable();

        Map(x => x.TipMerenja)
            .Column("TIP_MERENJA")
            .Nullable();

        Map(x => x.TipIzvora)
            .Column("TIP_IZVORA")
            .Nullable();

        Map(x => x.IsValidirano)
            .Column("IS_VALIDIRANO")
            .Not.Nullable();

        Map(x => x.Komentar)
            .Column("KOMENTAR")
            .Nullable();
    }
}
