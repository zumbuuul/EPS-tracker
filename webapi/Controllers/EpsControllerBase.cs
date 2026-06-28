using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [ApiController]
    public abstract class EpsControllerBase : ControllerBase
    {
        protected async Task<ActionResult<T>> Run<T>(Func<Task<T>> action)
        {
            try
            {
                return Ok(await action());
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        protected async Task<IActionResult> Run(Func<Task> action)
        {
            try
            {
                await action();
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        protected ActionResult HandleError(Exception ex)
        {
            if (ex is KeyNotFoundException)
            {
                return NotFound(new { message = ex.Message });
            }

            if (ex is ArgumentException)
            {
                return BadRequest(new { message = ex.Message });
            }

            if (ex is InvalidOperationException)
            {
                return Conflict(new { message = ex.Message });
            }

            return StatusCode(500, new { message = "Doslo je do greske na serveru." });
        }
    }
}
