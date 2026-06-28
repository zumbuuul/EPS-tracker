using System.Threading.Tasks;
using app.DTO;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/potrosaci")]
    public class PotrosaciController : EpsControllerBase
    {
        private readonly PotrosacService potrosacService;

        public PotrosaciController(PotrosacService potrosacService)
        {
            this.potrosacService = potrosacService;
        }

        [HttpGet]
        public Task<ActionResult<System.Collections.Generic.IList<PotrosacListDto>>> VratiSve()
        {
            return Run(() => potrosacService.VratiPotrosace());
        }

        [HttpGet("{id:long}")]
        public Task<ActionResult<PotrosacDto>> Vrati(long id)
        {
            return Run(() => potrosacService.VratiPotrosaca(id));
        }

        [HttpGet("{id:long}/brojila")]
        public Task<ActionResult<System.Collections.Generic.IList<BrojiloListDto>>> VratiBrojila(long id)
        {
            return Run(() => potrosacService.VratiBrojilaZaPotrosaca(id));
        }

        [HttpPost]
        public async Task<IActionResult> Dodaj([FromBody] PotrosacSaveDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Telo zahteva je obavezno." });
            }

            try
            {
                var id = await potrosacService.DodajPotrosaca(dto);
                return CreatedAtAction(nameof(Vrati), new { id }, new { id });
            }
            catch (System.Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPut("{id:long}")]
        public Task<IActionResult> Izmeni(long id, [FromBody] PotrosacSaveDto dto)
        {
            if (dto == null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new { message = "Telo zahteva je obavezno." }));
            }

            dto.Id = id;
            return Run(() => potrosacService.IzmeniPotrosaca(dto));
        }

        [HttpDelete("{id:long}")]
        public Task<IActionResult> Obrisi(long id)
        {
            return Run(() => potrosacService.ObrisiPotrosaca(id));
        }

        [HttpPost("{id:long}/brojila")]
        public Task<IActionResult> PoveziBrojilo(long id, [FromBody] PotrosacBrojiloLinkDto dto)
        {
            if (dto == null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new { message = "Telo zahteva je obavezno." }));
            }

            dto.PotrosacId = id;
            return Run(() => potrosacService.PoveziPotrosacaIBrojilo(dto));
        }

        [HttpDelete("{id:long}/brojila/{serijskiBroj}")]
        public Task<IActionResult> RaskiniBrojilo(long id, string serijskiBroj)
        {
            return Run(() => potrosacService.RaskiniVezuPotrosacBrojilo(id, serijskiBroj));
        }
    }
}
