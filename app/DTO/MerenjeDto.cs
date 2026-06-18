using System;
using app.Entities.Enums;

namespace app.DTO
{
    public class MerenjeListDto
    {
        public long Id { get; set; }

        public string SerijskiBroj { get; set; }

        public DateTime DatumVremeMerenja { get; set; }

        public decimal? PotrosnjaAktivna { get; set; }

        public decimal? PotrosnjaReaktivna { get; set; }

        public decimal? Snaga { get; set; }

        public decimal? Napon { get; set; }

        public TipIzvoraMerenja? TipIzvora { get; set; }

        public DaNe IsValidirano { get; set; }
    }

    public class MerenjeDto
    {
        public long Id { get; set; }

        public string SerijskiBroj { get; set; }

        public DateTime DatumVremeMerenja { get; set; }

        public decimal? PotrosnjaAktivna { get; set; }

        public decimal? PotrosnjaReaktivna { get; set; }

        public decimal? Snaga { get; set; }

        public decimal? Napon { get; set; }

        public decimal? Struja { get; set; }

        public string TipMerenja { get; set; }

        public TipIzvoraMerenja? TipIzvora { get; set; }

        public DaNe IsValidirano { get; set; }

        public string Komentar { get; set; }
    }

    public class MerenjeSaveDto
    {
        public long Id { get; set; }

        public string SerijskiBroj { get; set; }

        public DateTime DatumVremeMerenja { get; set; }

        public decimal? PotrosnjaAktivna { get; set; }

        public decimal? PotrosnjaReaktivna { get; set; }

        public decimal? Snaga { get; set; }

        public decimal? Napon { get; set; }

        public decimal? Struja { get; set; }

        public string TipMerenja { get; set; }

        public TipIzvoraMerenja? TipIzvora { get; set; }

        public DaNe IsValidirano { get; set; }

        public string Komentar { get; set; }
    }

    public class ValidacijaMerenjaDto
    {
        public long MerenjeId { get; set; }

        public DaNe IsValidirano { get; set; }

        public string Komentar { get; set; }
    }
}
