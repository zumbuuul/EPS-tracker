using System;

namespace app.DTO
{
    public class KvarListDto
    {
        public long Id { get; set; }

        public string SerijskiBroj { get; set; }

        public long PotrosacId { get; set; }

        public string ImeIliNazivPotrosaca { get; set; }

        public DateTime DatumPrijave { get; set; }

        public string TipKvara { get; set; }

        public string Status { get; set; }

        public string Prioritet { get; set; }

        public string NadlezniTim { get; set; }

        public decimal? TrajanjeUSatima { get; set; }

        public DateTime? DatumOtklanjanja { get; set; }
    }

    public class KvarDto
    {
        public long Id { get; set; }

        public string SerijskiBroj { get; set; }

        public long PotrosacId { get; set; }

        public string ImeIliNazivPotrosaca { get; set; }

        public DateTime DatumPrijave { get; set; }

        public string TipKvara { get; set; }

        public string OpisProblema { get; set; }

        public string Status { get; set; }

        public DateTime? DatumOtklanjanja { get; set; }

        public decimal? TrajanjeUSatima { get; set; }

        public string Prioritet { get; set; }

        public string NadlezniTim { get; set; }

        public string Komentar { get; set; }
    }

    public class KvarSaveDto
    {
        public long Id { get; set; }

        public string SerijskiBroj { get; set; }

        public long PotrosacId { get; set; }

        public DateTime DatumPrijave { get; set; }

        public string TipKvara { get; set; }

        public string OpisProblema { get; set; }

        public string Status { get; set; }

        public DateTime? DatumOtklanjanja { get; set; }

        public decimal? TrajanjeUSatima { get; set; }

        public string Prioritet { get; set; }

        public string NadlezniTim { get; set; }

        public string Komentar { get; set; }
    }

    public class OtkloniKvarDto
    {
        public long KvarId { get; set; }

        public DateTime DatumOtklanjanja { get; set; }

        public decimal? TrajanjeUSatima { get; set; }

        public string Komentar { get; set; }
    }
}
