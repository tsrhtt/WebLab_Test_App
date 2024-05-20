using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApi.Services;
using MyApi.Factories;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebLabController : ControllerBase
    {
        private readonly ILogger<WebLabController> _logger;
        private readonly WebLabService _webLabService;

        public WebLabController(ILogger<WebLabController> logger, WebLabServiceFactory serviceFactory)
        {
            _logger = logger;
            try
            {
                _webLabService = serviceFactory.CreateAsync().Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create WebLabService");
                throw;
            }
        }

        [HttpGet("direction")]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Authorization: " + Request.Headers["Authorization"]);
            Console.WriteLine("Authorization: " + Request.Headers["Authorization"]);
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

        [HttpGet("direction/detailed/{id}")]
        public async Task<IActionResult> GetDetailed(int id)
        {
            try
            {
                var direction = await _webLabService.GetDetailedDirection(id);
                return Ok(direction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting detailed direction");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
