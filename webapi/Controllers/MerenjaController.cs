using System.Collections.Generic;
using System.Threading.Tasks;
using app.DTO;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/merenja")]
    public class MerenjaController : EpsControllerBase
    {
        private readonly MerenjeService merenjeService;
        private readonly RacunService racunService;

        public MerenjaController(MerenjeService merenjeService, RacunService racunService)
        {
            this.merenjeService = merenjeService;
            this.racunService = racunService;
        }

        [HttpGet]
        public Task<ActionResult<IList<MerenjeListDto>>> VratiSve()
        {
            return Run(() => merenjeService.VratiMerenja());
        }

        [HttpGet("{id:long}")]
        public Task<ActionResult<MerenjeDto>> Vrati(long id)
        {
            return Run(() => merenjeService.VratiMerenje(id));
        }

        [HttpGet("{id:long}/racun")]
        public async Task<ActionResult<RacunDto>> VratiRacun(long id)
        {
            try
            {
                var racun = await racunService.VratiRacunZaMerenje(id);
                if (racun == null)
                {
                    return NotFound(new { message = "Racun za merenje nije pronadjen." });
                }

                return Ok(racun);
            }
            catch (System.Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Dodaj([FromBody] MerenjeSaveDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Telo zahteva je obavezno." });
            }

            try
            {
                var id = await merenjeService.DodajMerenje(dto);
                return CreatedAtAction(nameof(Vrati), new { id }, new { id });
            }
            catch (System.Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPut("{id:long}")]
        public Task<IActionResult> Izmeni(long id, [FromBody] MerenjeSaveDto dto)
        {
            if (dto == null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new { message = "Telo zahteva je obavezno." }));
            }

            dto.Id = id;
            return Run(() => merenjeService.IzmeniMerenje(dto));
        }

        [HttpDelete("{id:long}")]
        public Task<IActionResult> Obrisi(long id)
        {
            return Run(() => merenjeService.ObrisiMerenje(id));
        }
    }
}
