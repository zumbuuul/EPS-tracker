using System.Collections.Generic;
using System.Threading.Tasks;
using app.DTO;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/brojila")]
    public class BrojilaController : EpsControllerBase
    {
        private readonly BrojiloService brojiloService;

        public BrojilaController(BrojiloService brojiloService)
        {
            this.brojiloService = brojiloService;
        }

        [HttpGet]
        public Task<ActionResult<IList<BrojiloListDto>>> VratiSve()
        {
            return Run(() => brojiloService.VratiBrojila());
        }

        [HttpGet("{serijskiBroj}")]
        public Task<ActionResult<BrojiloDto>> Vrati(string serijskiBroj)
        {
            return Run(() => brojiloService.VratiBrojilo(serijskiBroj));
        }

        [HttpGet("{serijskiBroj}/merenja")]
        public Task<ActionResult<IList<MerenjeListDto>>> VratiMerenja(string serijskiBroj)
        {
            return Run(() => brojiloService.VratiMerenjaZaBrojilo(serijskiBroj));
        }

        [HttpGet("{serijskiBroj}/kvarovi")]
        public Task<ActionResult<IList<KvarListDto>>> VratiKvarove(string serijskiBroj)
        {
            return Run(() => brojiloService.VratiKvaroveZaBrojilo(serijskiBroj));
        }

        [HttpPost]
        public async Task<IActionResult> Dodaj([FromBody] BrojiloSaveDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Telo zahteva je obavezno." });
            }

            try
            {
                await brojiloService.DodajBrojilo(dto);
                return CreatedAtAction(nameof(Vrati), new { serijskiBroj = dto.SerijskiBroj }, new { dto.SerijskiBroj });
            }
            catch (System.Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPut("{serijskiBroj}")]
        public Task<IActionResult> Izmeni(string serijskiBroj, [FromBody] BrojiloSaveDto dto)
        {
            if (dto == null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new { message = "Telo zahteva je obavezno." }));
            }

            dto.SerijskiBroj = serijskiBroj;
            return Run(() => brojiloService.IzmeniBrojilo(dto));
        }

        [HttpDelete("{serijskiBroj}")]
        public Task<IActionResult> Obrisi(string serijskiBroj)
        {
            return Run(() => brojiloService.ObrisiBrojilo(serijskiBroj));
        }
    }
}
