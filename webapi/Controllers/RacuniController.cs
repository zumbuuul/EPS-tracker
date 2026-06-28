using System.Collections.Generic;
using System.Threading.Tasks;
using app.DTO;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/racuni")]
    public class RacuniController : EpsControllerBase
    {
        private readonly RacunService racunService;

        public RacuniController(RacunService racunService)
        {
            this.racunService = racunService;
        }

        [HttpGet]
        public Task<ActionResult<IList<RacunListDto>>> VratiSve()
        {
            return Run(() => racunService.VratiRacune());
        }

        [HttpGet("{brojRacuna}")]
        public Task<ActionResult<RacunDto>> Vrati(string brojRacuna)
        {
            return Run(() => racunService.VratiRacun(brojRacuna));
        }

        [HttpPut("{brojRacuna}")]
        public Task<IActionResult> Izmeni(string brojRacuna, [FromBody] RacunSaveDto dto)
        {
            if (dto == null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new { message = "Telo zahteva je obavezno." }));
            }

            dto.BrojRacuna = brojRacuna;
            return Run(() => racunService.IzmeniRacun(dto));
        }

        [HttpDelete("{brojRacuna}")]
        public Task<IActionResult> Obrisi(string brojRacuna)
        {
            return Run(() => racunService.ObrisiRacun(brojRacuna));
        }
    }
}
