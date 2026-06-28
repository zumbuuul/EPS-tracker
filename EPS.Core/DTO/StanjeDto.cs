using System;

namespace app.DTO
{
    public class StanjeListDto
    {
        public long Id { get; set; }

        public DateTime DatumIVreme { get; set; }

        public string Lokacija { get; set; }

        public string Transformator { get; set; }

        public decimal NaponskiNivo { get; set; }

        public decimal UkupnaPotrosnja { get; set; }

        public decimal Gubitak { get; set; }

        public decimal DistribuiranaEnergija { get; set; }

        public decimal ProizvedenaEnergija { get; set; }

        public string Status { get; set; }
    }

    public class StanjeDto
    {
        public long Id { get; set; }

        public DateTime DatumIVreme { get; set; }

        public string Lokacija { get; set; }

        public decimal NaponskiNivo { get; set; }

        public string Status { get; set; }

        public string Transformator { get; set; }

        public string Komentar { get; set; }

        public decimal UkupnaPotrosnja { get; set; }

        public decimal Gubitak { get; set; }

        public decimal DistribuiranaEnergija { get; set; }

        public decimal ProizvedenaEnergija { get; set; }
    }

    public class StanjeSaveDto
    {
        public long Id { get; set; }

        public DateTime DatumIVreme { get; set; }

        public string Lokacija { get; set; }

        public decimal NaponskiNivo { get; set; }

        public string Status { get; set; }

        public string Transformator { get; set; }

        public string Komentar { get; set; }

        public decimal UkupnaPotrosnja { get; set; }

        public decimal Gubitak { get; set; }

        public decimal DistribuiranaEnergija { get; set; }

        public decimal ProizvedenaEnergija { get; set; }
    }
}
