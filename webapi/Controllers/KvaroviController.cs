using System.Collections.Generic;
using System.Threading.Tasks;
using app.DTO;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/kvarovi")]
    public class KvaroviController : EpsControllerBase
    {
        private readonly KvarService kvarService;

        public KvaroviController(KvarService kvarService)
        {
            this.kvarService = kvarService;
        }

        [HttpGet]
        public Task<ActionResult<IList<KvarListDto>>> VratiSve()
        {
            return Run(() => kvarService.VratiKvarove());
        }

        [HttpGet("{id:long}")]
        public Task<ActionResult<KvarDto>> Vrati(long id)
        {
            return Run(() => kvarService.VratiKvar(id));
        }

        [HttpPost]
        public async Task<IActionResult> Dodaj([FromBody] KvarSaveDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Telo zahteva je obavezno." });
            }

            try
            {
                var id = await kvarService.DodajKvar(dto);
                return CreatedAtAction(nameof(Vrati), new { id }, new { id });
            }
            catch (System.Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPut("{id:long}")]
        public Task<IActionResult> Izmeni(long id, [FromBody] KvarSaveDto dto)
        {
            if (dto == null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new { message = "Telo zahteva je obavezno." }));
            }

            dto.Id = id;
            return Run(() => kvarService.IzmeniKvar(dto));
        }

        [HttpDelete("{id:long}")]
        public Task<IActionResult> Obrisi(long id)
        {
            return Run(() => kvarService.ObrisiKvar(id));
        }

        [HttpPost("{id:long}/otkloni")]
        public Task<IActionResult> Otkloni(long id, [FromBody] OtkloniKvarDto? dto = null)
        {
            if (dto == null)
            {
                return Run(() => kvarService.OtkloniKvar(id));
            }

            dto.KvarId = id;
            return Run(() => kvarService.OtkloniKvar(dto));
        }
    }
}
