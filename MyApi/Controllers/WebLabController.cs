using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApi.Services;
using MyApi.Factories;
using MyApi.DTOs;

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

        [HttpGet("direction/fromdb")]
        public async Task<IActionResult> GetFromDb()
        {
            try
            {
                var directions = await _webLabService.GetDirectionsFromDb();
                return Ok(directions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting directions from database");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("direction/{id}")]
        public async Task<IActionResult> UpdateDirection(int id, [FromBody] DirectionDto directionDto)
        {
            if (id != directionDto.Id)
            {
                return BadRequest("Direction ID mismatch.");
            }

            try
            {
                await _webLabService.UpdateDirection(directionDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating direction");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("direction/{id}")]
        public async Task<IActionResult> DeleteDirection(int id)
        {
            try
            {
                await _webLabService.DeleteDirection(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting direction");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("direction/accept/{id}")]
        public async Task<IActionResult> AcceptDirection(int id, [FromBody] AcceptDirectionRequest request)
        {
            try
            {
                await _webLabService.AcceptDirection(id, request.AcceptedBy, request.Comment);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting direction");
                return StatusCode(500, "Internal server error");
            }
        }

        public class AcceptDirectionRequest
        {
            public string? AcceptedBy { get; set; }
            public string? Comment { get; set; }
        }
    }
}
