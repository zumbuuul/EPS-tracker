using app.Entities;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class StanjeMap : ClassMap<Stanje>
    {
        public StanjeMap()
        {
            Table("STANJE");

            Id(x => x.Id)
                .Column("ID")
                .GeneratedBy.Identity();

            Map(x => x.DatumIVreme)
                .Column("DATUM_I_VREME")
                .Not.Nullable();

            Map(x => x.Lokacija)
                .Column("LOKACIJA")
                .Length(200)
                .Not.Nullable();

            Map(x => x.NaponskiNivo)
                .Column("NAPONSKI_NIVO")
                .Precision(10)
                .Scale(2)
                .Not.Nullable();

            Map(x => x.Status)
                .Column("STATUS")
                .Length(50)
                .Not.Nullable();

            Map(x => x.Transformator)
                .Column("TRANSFORMATOR")
                .Length(200)
                .Not.Nullable();

            Map(x => x.Komentar)
                .Column("KOMENTAR")
                .Length(500)
                .Nullable();

            Map(x => x.UkupnaPotrosnja)
                .Column("UKUPNA_POTROSNJA")
                .Precision(15)
                .Scale(4)
                .Not.Nullable();

            Map(x => x.Gubitak)
                .Column("GUBITAK")
                .Precision(15)
                .Scale(4)
                .Not.Nullable();

            Map(x => x.DistribuiranaEnergija)
                .Column("DISTRIBUIRANA_ENERGIJA")
                .Precision(15)
                .Scale(4)
                .Not.Nullable();

            Map(x => x.ProizvedenaEnergija)
                .Column("PROIZVEDENA_ENERGIJA")
                .Precision(15)
                .Scale(4)
                .Not.Nullable();
        }
    }
}
