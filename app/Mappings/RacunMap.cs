using app.Entities;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class RacunMap : ClassMap<Racun>
    {
        public RacunMap()
        {
            Table("RACUN");
            Not.LazyLoad();

            Id(x => x.BrojRacuna)
                .Column("BROJ_RACUNA")
                .Length(50)
                .GeneratedBy.Assigned();

            Map(x => x.DatumIzdavanja)
                .Column("DATUM_IZDAVANJA")
                .Not.Nullable();

            Map(x => x.RokPlacanja)
                .Column("ROK_PLACANJA")
                .Not.Nullable();

            Map(x => x.Komentar)
                .Column("KOMENTAR")
                .Length(500)
                .Nullable();

            Map(x => x.NacinPlacanja)
                .Column("NACIN_PLACANJA")
                .Length(50)
                .Nullable();

            Map(x => x.PeriodPotrosnjeOd)
                .Column("PERIOD_POTROSNJE_OD")
                .Not.Nullable();

            Map(x => x.PeriodPotrosnjeDo)
                .Column("PERIOD_POTROSNJE_DO")
                .Not.Nullable();

            Map(x => x.IznosBezPdv)
                .Column("IZNOS_BEZ_PDV")
                .Precision(15)
                .Scale(2)
                .Not.Nullable();

            Map(x => x.Pdv)
                .Column("PDV")
                .Precision(15)
                .Scale(2)
                .Not.Nullable();

            Map(x => x.Status)
                .Column("STATUS")
                .Length(50)
                .Not.Nullable();

            References(x => x.Merenje)
                .Column("MERENJE_ID")
                .Not.Nullable();

            References(x => x.Potrosac)
                .Column("POTROSAC_ID")
                .Not.Nullable();
        }
    }
}
