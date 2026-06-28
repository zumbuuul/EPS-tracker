using System;
using System.Collections.Generic;
<<<<<<< HEAD
using app.Entities.Enums;
=======
using System.Linq;
using System.Text;
>>>>>>> 30cdadda99c4fff3b2e853e691e1a31b640397c5

namespace app.DTO
{
    public class BrojiloListDto
    {
        public string SerijskiBroj { get; set; }

        public IList<TipBrojila> TipoviBrojila { get; set; }

        public DateTime DatumInstalacije { get; set; }

        public DateTime? PoslednjiDatumZamene { get; set; }

        public string Status { get; set; }

        public string Lokacija { get; set; }

        public decimal? KoeficijentMnozenja { get; set; }

        public int BrojPotrosaca { get; set; }

        public BrojiloListDto()
        {
            TipoviBrojila = new List<TipBrojila>();
        }
    }

    public class BrojiloDto
    {
<<<<<<< HEAD
        public BrojiloDto()
        {
            TipoviBrojila = new List<TipBrojila>();
            DatumiZamene = new List<DateTime>();
            Potrosaci = new List<PotrosacListDto>();
            Merenja = new List<MerenjeListDto>();
            Kvarovi = new List<KvarListDto>();
        }

        public string SerijskiBroj { get; set; }

        public DateTime DatumInstalacije { get; set; }

        public string Status { get; set; }

        public string Lokacija { get; set; }

        public decimal? KoeficijentMnozenja { get; set; }

        public string Komentar { get; set; }

        public IList<TipBrojila> TipoviBrojila { get; set; }

        public IList<DateTime> DatumiZamene { get; set; }

        public MehanickoBrojiloDto Mehanicko { get; set; }

        public PametnoBrojiloDto Pametno { get; set; }

        public TrofaznoBrojiloDto Trofazno { get; set; }

        public RasvetnoBrojiloDto Rasvetno { get; set; }

        public IList<PotrosacListDto> Potrosaci { get; set; }

        public IList<MerenjeListDto> Merenja { get; set; }

        public IList<KvarListDto> Kvarovi { get; set; }
    }

    public class BrojiloSaveDto
    {
        public BrojiloSaveDto()
        {
            TipoviBrojila = new List<TipBrojila>();
            DatumiZamene = new List<DateTime>();
        }

        public string SerijskiBroj { get; set; }

        public DateTime DatumInstalacije { get; set; }

        public string Status { get; set; }

        public string Lokacija { get; set; }

        public decimal? KoeficijentMnozenja { get; set; }

        public string Komentar { get; set; }

        public IList<TipBrojila> TipoviBrojila { get; set; }

        public IList<DateTime> DatumiZamene { get; set; }

        public MehanickoBrojiloDto Mehanicko { get; set; }

        public PametnoBrojiloDto Pametno { get; set; }

        public TrofaznoBrojiloDto Trofazno { get; set; }

        public RasvetnoBrojiloDto Rasvetno { get; set; }
    }

    public class MehanickoBrojiloDto
    {
        public DateTime? PoslednjaKalibracija { get; set; }

        public decimal? MaxGreska { get; set; }

        public string Preciznost { get; set; }
    }

    public class PametnoBrojiloDto
    {
        public PametnoBrojiloProtokol? Protokol { get; set; }

        public decimal? Frekvencija { get; set; }

        public DaNe IsDaljinskoIskljucenje { get; set; }

        public decimal? NivoBaterije { get; set; }
    }

    public class TrofaznoBrojiloDto
    {
        public decimal? MaxSnaga { get; set; }

        public DaNe MogucnostMerenjaPoZonama { get; set; }

        public decimal? UgovorenaSnaga { get; set; }
    }

    public class RasvetnoBrojiloDto
    {
        public string VremeUkljucenja { get; set; }

        public string VremeIskljucenja { get; set; }

        public string StatusSenzora { get; set; }

        public decimal? RadnoVreme { get; set; }
=======
        public string SerijskiBroj { get; set; }
        public DateTime DatumInstalacije { get; set; }
        public string Status { get; set; }
        public string Lokacija { get; set; }
        public decimal? KoeficijentMnozenja { get; set; }

        public string Tipovi { get; set; }
        public string Potrosaci { get; set; }
        public string DatumiZamene { get; set; }
>>>>>>> 30cdadda99c4fff3b2e853e691e1a31b640397c5
    }
}
