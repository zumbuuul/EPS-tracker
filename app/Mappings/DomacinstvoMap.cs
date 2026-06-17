using app.Entities;
using FluentNHibernate.Mapping;

namespace app.Mappings
{
    public sealed class DomacinstvoMap : ClassMap<Domacinstvo>
    {
        public DomacinstvoMap()
        {
            Table("DOMACINSTVO");

            Id(x => x.Id)
                .Column("ID")
                .GeneratedBy.Foreign("Potrosac");

            HasOne(x => x.Potrosac)
                .Constrained();

            Map(x => x.Jmbg)
                .Column("JMBG")
                .Length(13)
                .Unique()
                .Not.Nullable();

            Map(x => x.Ime)
                .Column("IME")
                .Length(100)
                .Not.Nullable();

            Map(x => x.Prezime)
                .Column("PREZIME")
                .Length(100)
                .Not.Nullable();
        }
    }
}
