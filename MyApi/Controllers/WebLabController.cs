using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebLabController : ControllerBase
    {
        private readonly ILogger<WebLabController> _logger;
        private readonly WebLabService _webLabService;

        public WebLabController(ILogger<WebLabController> logger, WebLabService webLabService)
        {
            _logger = logger;
            _webLabService = webLabService;
        }

        [HttpGet("direction")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var labData = await _webLabService.GetLabData();
                return Ok(labData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lab data");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
