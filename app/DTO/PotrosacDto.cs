using System.Collections.Generic;
using app.Entities.Enums;

namespace app.DTO
{
    public class PotrosacListDto
    {
        public long Id { get; set; }

        public PotrosacTip Tip { get; set; }

        public string ImeIliNaziv { get; set; }

        public string Grad { get; set; }

        public string Telefon { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public string KategorijaTarife { get; set; }

        public int BrojBrojila { get; set; }
    }

    public class PotrosacDto
    {
        public PotrosacDto()
        {
            Brojila = new List<BrojiloListDto>();
            Racuni = new List<RacunListDto>();
            Kvarovi = new List<KvarListDto>();
        }

        public long Id { get; set; }

        public PotrosacTip Tip { get; set; }

        public string Email { get; set; }

        public string Telefon { get; set; }

        public string Adresa { get; set; }

        public string Grad { get; set; }

        public string Komentar { get; set; }

        public string Status { get; set; }

        public string KategorijaTarife { get; set; }

        public DomacinstvoDto Domacinstvo { get; set; }

        public FirmaDto Firma { get; set; }

        public IList<BrojiloListDto> Brojila { get; set; }

        public IList<RacunListDto> Racuni { get; set; }

        public IList<KvarListDto> Kvarovi { get; set; }
    }

    public class PotrosacSaveDto
    {
        public long Id { get; set; }

        public PotrosacTip Tip { get; set; }

        public string Email { get; set; }

        public string Telefon { get; set; }

        public string Adresa { get; set; }

        public string Grad { get; set; }

        public string Komentar { get; set; }

        public string Status { get; set; }

        public string KategorijaTarife { get; set; }

        public DomacinstvoDto Domacinstvo { get; set; }

        public FirmaDto Firma { get; set; }
    }

    public class DomacinstvoDto
    {
        public long Id { get; set; }

        public string Jmbg { get; set; }

        public string Ime { get; set; }

        public string Prezime { get; set; }
    }

    public class FirmaDto
    {
        public long Id { get; set; }

        public string Naziv { get; set; }

        public string Pib { get; set; }
    }

    public class PotrosacBrojiloLinkDto
    {
        public long PotrosacId { get; set; }

        public string SerijskiBroj { get; set; }
    }
}
