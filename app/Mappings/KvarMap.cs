using app.Entities;
using app.Entities.Enums;
using app.Mappings.Types;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class KvarMap : ClassMap<Kvar>
    {
        public KvarMap()
        {
            Table("KVAR");
            Not.LazyLoad();

            Id(x => x.Id)
                .Column("ID")
                .GeneratedBy.TriggerIdentity();

            References(x => x.Brojilo)
                .Column("SERIJSKI_BROJ")
                .Not.Nullable();

            References(x => x.Potrosac)
                .Column("POTROSAC_ID")
                .Not.Nullable();

            Map(x => x.DatumPrijave)
                .Column("DATUM_PRIJAVE")
                .Not.Nullable();

            Map(x => x.TipKvara)
                .Column("TIP_KVARA")
                .Length(100)
                .Not.Nullable();

            Map(x => x.OpisProblema)
                .Column("OPIS_PROBLEMA")
                .Length(1000)
                .Nullable();

            Map(x => x.Status)
                .Column("STATUS")
                .CustomType<DdlEnumStringType<KvarStatus>>()
                .Length(50)
                .Not.Nullable();

            Map(x => x.DatumOtklanjanja)
                .Column("DATUM_OTKLANJANJA")
                .Nullable();

            Map(x => x.TrajanjeUSatima)
                .Column("TRAJANJE_U_SATIMA")
                .Precision(10)
                .Scale(2)
                .Nullable();

            Map(x => x.Prioritet)
                .Column("PRIORITET")
                .CustomType<DdlEnumStringType<KvarPrioritet>>()
                .Length(20)
                .Nullable();

            Map(x => x.Komentar)
                .Column("KOMENTAR")
                .Length(500)
                .Nullable();

            Map(x => x.NadlezniTim)
                .Column("NADLEZNI_TIM")
                .Length(200)
                .Nullable();
        }
    }
}
