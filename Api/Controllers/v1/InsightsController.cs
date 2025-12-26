using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;
using WeatherApi.Infrastructure.Services;

namespace WeatherApi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/insights")]
    public class InsightsController : ControllerBase
    {
        private readonly ITodaySummaryService _todaySummaryService;
        private readonly ITravelScoreService _travelScoreService;

        public InsightsController(ITodaySummaryService todaySummaryService, ITravelScoreService travelScoreService)
        {
            _todaySummaryService = todaySummaryService;
            _travelScoreService = travelScoreService;
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

        [HttpGet("explain/travel-score/{city}")]
        [ProducesResponseType(typeof(TravelScoreExplanationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExplainTravelScore(
        string city,
        CancellationToken cancellationToken)
        {
            var result = await _travelScoreService
                .ExplainTravelScoreAsync(city, cancellationToken);

            return result == null
                ? NotFound()
                : Ok(result);
        }
    }
}
