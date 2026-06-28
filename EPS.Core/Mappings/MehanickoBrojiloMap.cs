using app.Entities;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class MehanickoBrojiloMap : ClassMap<MehanickoBrojilo>
    {
        public MehanickoBrojiloMap()
        {
            Table("MEHANICKO");
            Not.LazyLoad();

            Id(x => x.SerijskiBroj)
                .Column("SERIJSKI_BROJ")
                .Length(50)
                .GeneratedBy.Foreign("Brojilo");

            HasOne(x => x.Brojilo)
                .Constrained();

            Map(x => x.PoslednjaKalibracija)
                .Column("POSLEDNJA_KALIBRACIJA")
                .Nullable();

            Map(x => x.MaxGreska)
                .Column("MAX_GRESKA")
                .Precision(10)
                .Scale(4)
                .Nullable();

            Map(x => x.Preciznost)
                .Column("PRECIZNOST")
                .Length(50)
                .Nullable();
        }
    }
}
