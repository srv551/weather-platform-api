using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Application.Domain;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/health")]
    public class HealthController : ControllerBase
    {
        private readonly IHealthWeatherService _service;

        public HealthController(IHealthWeatherService service)
        {
            _service = service;
        }

        /// <summary>
        /// Returns health-aware weather insights for a specific condition.
        /// </summary>
        [HttpGet("{city}")]
        public async Task<IActionResult> GetHealthInsight(
            string city,
            [FromQuery] HealthConditionType condition,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetHealthInsightAsync(city, condition, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
