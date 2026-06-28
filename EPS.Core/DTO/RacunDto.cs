using System;

namespace app.DTO
{
    public class RacunListDto
    {
        public string BrojRacuna { get; set; }

        public long PotrosacId { get; set; }

        public string ImeIliNazivPotrosaca { get; set; }

        public string SerijskiBroj { get; set; }

        public DateTime PeriodPotrosnjeOd { get; set; }

        public DateTime PeriodPotrosnjeDo { get; set; }

        public decimal? UkupnaPotrosnja { get; set; }

        public decimal IznosBezPdv { get; set; }

        public decimal Pdv { get; set; }

        public decimal UkupanIznos { get; set; }

        public DateTime DatumIzdavanja { get; set; }

        public DateTime RokPlacanja { get; set; }

        public string Status { get; set; }

        public string NacinPlacanja { get; set; }
    }

    public class RacunDto
    {
        public string BrojRacuna { get; set; }

        public long PotrosacId { get; set; }

        public string ImeIliNazivPotrosaca { get; set; }

        public string SerijskiBroj { get; set; }

        public long MerenjeId { get; set; }

        public DateTime DatumIzdavanja { get; set; }

        public DateTime RokPlacanja { get; set; }

        public DateTime PeriodPotrosnjeOd { get; set; }

        public DateTime PeriodPotrosnjeDo { get; set; }

        public decimal? UkupnaPotrosnja { get; set; }

        public decimal IznosBezPdv { get; set; }

        public decimal Pdv { get; set; }

        public decimal UkupanIznos { get; set; }

        public string Status { get; set; }

        public string NacinPlacanja { get; set; }

        public string Komentar { get; set; }
    }

    public class RacunSaveDto
    {
        public string BrojRacuna { get; set; }

        public long PotrosacId { get; set; }

        public long MerenjeId { get; set; }

        public DateTime DatumIzdavanja { get; set; }

        public DateTime RokPlacanja { get; set; }

        public DateTime PeriodPotrosnjeOd { get; set; }

        public DateTime PeriodPotrosnjeDo { get; set; }

        public decimal IznosBezPdv { get; set; }

        public decimal Pdv { get; set; }

        public string Status { get; set; }

        public string NacinPlacanja { get; set; }

        public string Komentar { get; set; }
    }

    public class GenerisiRacunDto
    {
        public long PotrosacId { get; set; }

        public long MerenjeId { get; set; }

        public DateTime PeriodPotrosnjeOd { get; set; }

        public DateTime PeriodPotrosnjeDo { get; set; }

        public string NacinPlacanja { get; set; }
    }
}
