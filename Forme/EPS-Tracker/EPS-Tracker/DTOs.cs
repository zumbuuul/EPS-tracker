namespace EPS_Tracker;

public class PotrosacPregled
{
    public long Id { get; set; }
    public string Tip { get; set; }
    public string Email { get; set; }
    public string Telefon { get; set; }
    public string Adresa { get; set; }
    public string Grad { get; set; }
    public string Status { get; set; }

    public PotrosacPregled(long id, string tip, string email, string telefon, string adresa, string grad, string status)
    {
        Id = id;
        Tip = tip;
        Email = email;
        Telefon = telefon;
        Adresa = adresa;
        Grad = grad;
        Status = status;
    }

    public class BrojiloPregled
    {
        public string SerijskiBroj { get; set; }
        public DateTime DatumInstalacije { get; set; }
        public string Status { get; set; }
        public string Lokacija { get; set; }
        public decimal? KoeficijentMnozenja { get; set; }

        public BrojiloPregled(string serijskiBroj, DateTime datumInstalacije, string status, string lokacija, decimal? koeficijentMnozenja)
        {
            SerijskiBroj = serijskiBroj;
            DatumInstalacije = datumInstalacije;
            Status = status;
            Lokacija = lokacija;
            KoeficijentMnozenja = koeficijentMnozenja;
        }
    }

    public class PotrosacBasic
    {
        public long Id { get; set; }
        public string Tip { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefon { get; set; }
        public string Adresa { get; set; } = string.Empty;
        public string Grad { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string KategorijaTarife { get; set; } = string.Empty;
    }

    public class BrojiloBasic
    {
        public string SerijskiBroj { get; set; } = string.Empty;
        public DateTime DatumInstalacije { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Lokacija { get; set; }
        public decimal? KoeficijentMnozenja { get; set; }
        public string? Komentar { get; set; }
    }

    public class MerenjePregled
    {
        public long Id { get; set; }
        public string SerijskiBroj { get; set; }
        public DateTime DatumVremeMerenja { get; set; }
        public decimal? PotrosnjaAktivna { get; set; }
        public decimal? Snaga { get; set; }
        public decimal? Napon { get; set; }
        public string IsValidirano { get; set; }

        public MerenjePregled(long id, string serijskiBroj, DateTime datumVremeMerenja,
            decimal? potrosnjaAktivna, decimal? snaga, decimal? napon, string isValidirano)
        {
            Id = id;
            SerijskiBroj = serijskiBroj;
            DatumVremeMerenja = datumVremeMerenja;
            PotrosnjaAktivna = potrosnjaAktivna;
            Snaga = snaga;
            Napon = napon;
            IsValidirano = isValidirano;
        }
    }

    public class MerenjeBasic
    {
        public long Id { get; set; }
        public string SerijskiBroj { get; set; } = string.Empty;
        public DateTime DatumVremeMerenja { get; set; }
        public decimal? PotrosnjaAktivna { get; set; }
        public decimal? PotrosnjaReaktivna { get; set; }
        public decimal? Snaga { get; set; }
        public decimal? Napon { get; set; }
        public decimal? Struja { get; set; }
        public string? TipMerenja { get; set; }
        public string? TipIzvora { get; set; }
        public string IsValidirano { get; set; } = "N";
        public string? Komentar { get; set; }
    }
}