using System.Collections.Generic;
using System.Threading.Tasks;
using app.DTO;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/stanja")]
    public class StanjaController : EpsControllerBase
    {
        private readonly StanjeService stanjeService;

        public StanjaController(StanjeService stanjeService)
        {
            this.stanjeService = stanjeService;
        }

        [HttpGet]
        public Task<ActionResult<IList<StanjeListDto>>> VratiSve()
        {
            return Run(() => stanjeService.VratiStanja());
        }

        [HttpGet("{id:long}")]
        public Task<ActionResult<StanjeDto>> Vrati(long id)
        {
            return Run(() => stanjeService.VratiStanje(id));
        }

        [HttpPost]
        public async Task<IActionResult> Dodaj([FromBody] StanjeSaveDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Telo zahteva je obavezno." });
            }

            try
            {
                var id = await stanjeService.DodajStanje(dto);
                return CreatedAtAction(nameof(Vrati), new { id }, new { id });
            }
            catch (System.Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPut("{id:long}")]
        public Task<IActionResult> Izmeni(long id, [FromBody] StanjeSaveDto dto)
        {
            if (dto == null)
            {
                return Task.FromResult<IActionResult>(BadRequest(new { message = "Telo zahteva je obavezno." }));
            }

            dto.Id = id;
            return Run(() => stanjeService.IzmeniStanje(dto));
        }

        [HttpDelete("{id:long}")]
        public Task<IActionResult> Obrisi(long id)
        {
            return Run(() => stanjeService.ObrisiStanje(id));
        }
    }
}
