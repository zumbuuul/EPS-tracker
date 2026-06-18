using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.DTO
{
    public class BrojiloDto
    {
        public string SerijskiBroj { get; set; }
        public DateTime DatumInstalacije { get; set; }
        public string Status { get; set; }
        public string Lokacija { get; set; }
        public decimal? KoeficijentMnozenja { get; set; }

        public string Tipovi { get; set; }
        public string Potrosaci { get; set; }
        public string DatumiZamene { get; set; }
    }
}
