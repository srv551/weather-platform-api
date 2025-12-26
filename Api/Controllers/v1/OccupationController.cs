using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Application.Domain;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Api.Controllers.V1
{
    /// <summary>
    /// Provides occupation-specific weather insights.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/occupation")]
    public class OccupationController : ControllerBase
    {
        private readonly IOccupationWeatherService _service;

        public OccupationController(IOccupationWeatherService service)
        {
            _service = service;
        }

        /// <summary>
        /// Returns weather guidance tailored to a specific occupation.
        /// </summary>
        [HttpGet("{occupation}/{city}")]
        public async Task<IActionResult> Get(
            string occupation,
            string city,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetOccupationWeatherAsync(city, occupation, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("available")]
        public IActionResult GetAvailableOccupations()
        {
            return Ok(Enum.GetNames(typeof(OccupationType)));
        }

    }
}
