using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApi.Services;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebLabController : ControllerBase
    {
        private readonly WebLabService _service;

        public WebLabController(WebLabService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetDirections()
        {
            var directions = await _service.GetDirectionsAsync();
            if (directions != null)
            {
                return Ok(directions);
            }
            else
            {
                return NotFound();
            }

        }

    }
}
