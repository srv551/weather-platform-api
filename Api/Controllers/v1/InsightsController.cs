using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/insights")]
    public class InsightsController : ControllerBase
    {
        private readonly ITodaySummaryService _todaySummaryService;

        public InsightsController(ITodaySummaryService todaySummaryService)
        {
            _todaySummaryService = todaySummaryService;
        }

        /// <summary>
        /// Returns a human-friendly daily weather insight for a city.
        /// </summary>
        [HttpGet("daily/{city}")]
        [ProducesResponseType(typeof(DailyInsightResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDailyInsight(
            string city,
            CancellationToken cancellationToken)
        {
            var result = await _todaySummaryService.GetDailyInsightAsync(city, cancellationToken);

            return result == null
                ? NotFound()
                : Ok(result);
        }
    }
}
