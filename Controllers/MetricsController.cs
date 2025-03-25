using Microsoft.AspNetCore.Mvc;
using TapMangoChallenge.Services;

namespace TapMangoChallenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly MetricsService _metricsService;

        public MetricsController(MetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        // GET /metrics
        [HttpGet]
        public IActionResult GetMetrics([FromQuery] string phoneNumber, [FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var events = _metricsService.GetEvents(start, end, phoneNumber);
            var globalRate = _metricsService.GetGlobalRate(start, end);
            var perNumberRate = _metricsService.GetPerNumberRate(start, end);

            return Ok(new
            {
                globalRate,
                perNumberRate,
                events = events.ToList()
            });
        }
    }
}