using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Filters;
using WeatherApi.Api.Swagger.Examples;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Api.Controllers.v1
{
    /// <summary>
    /// Provides endpoints for current weather and forecast data.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [EnableRateLimiting("global")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly ITodaySummaryService _todaySummaryService;
        private readonly IWeatherAdviceService _weatherAdviceService;
        private readonly ITravelScoreService _travelScoreService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(
            IWeatherService weatherService,
            ITodaySummaryService todaySummaryService,
            IWeatherAdviceService weatherAdviceService,
            ITravelScoreService travelScoreService,
            ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _todaySummaryService = todaySummaryService;
            _weatherAdviceService = weatherAdviceService;
            _travelScoreService = travelScoreService;
            _logger = logger;
        }

        #region Helpers

        private ErrorResponse BuildError(string code, string message)
            => new()
            {
                Code = code,
                Message = message,
                TraceId = HttpContext.TraceIdentifier
            };

        private async Task<IActionResult> ExecuteAsync<T>(
            Func<CancellationToken, Task<T?>> operation,
            string notFoundMessage,
            string context,
            CancellationToken cancellationToken,
            bool mapHttpRequestExceptionTo502 = false) where T : class
        {
            try
            {
                var result = await operation(cancellationToken);
                if (result == null)
                {
                    _logger.LogInformation("NotFound in {Context}: {Message}", context, notFoundMessage);
                    return NotFound(BuildError("NOT_FOUND", notFoundMessage));
                }

                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Operation cancelled in {Context}", context);
                // 499 is used by some proxies to indicate client closed request; return literal if you want to keep this semantic.
                return StatusCode(499, BuildError("REQUEST_CANCELLED", "The request was cancelled."));
            }
            catch (HttpRequestException ex) when (mapHttpRequestExceptionTo502)
            {
                _logger.LogError(ex, "Upstream HTTP error in {Context}", context);
                return StatusCode(StatusCodes.Status502BadGateway, BuildError("UPSTREAM_ERROR", "Error calling external weather service."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in {Context}", context);
                return StatusCode(StatusCodes.Status500InternalServerError, BuildError("UNEXPECTED_ERROR", "An unexpected error occurred."));
            }
        }

        #endregion

        /// <summary>
        /// Gets the current weather for a given city.
        /// </summary>
        [HttpGet("city/{city}")]
        [ProducesResponseType(typeof(WeatherResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> GetCurrentByCity(string city, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return Task.FromResult<IActionResult>(BadRequest(BuildError("CITY_REQUIRED", "City is required.")));
            }

            return ExecuteAsync<WeatherResult>(
                ct => _weatherService.GetCurrentByCityAsync(city, ct),
                $"Weather not found for city '{city}'.",
                $"GetCurrentByCity for {city}",
                cancellationToken,
                mapHttpRequestExceptionTo502: true);
        }

        /// <summary>
        /// Gets the current weather for a given latitude and longitude.
        /// </summary>
        [HttpGet("coordinates")]
        [ProducesResponseType(typeof(WeatherResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> GetCurrentByCoordinates([FromQuery] double lat, [FromQuery] double lon, CancellationToken cancellationToken)
        {
            return ExecuteAsync<WeatherResult>(
                ct => _weatherService.GetCurrentByCoordinatesAsync(lat, lon, ct),
                $"Weather not found for coordinates ({lat}, {lon}).",
                $"GetCurrentByCoordinates for {lat},{lon}",
                cancellationToken,
                mapHttpRequestExceptionTo502: true);
        }

        /// <summary>
        /// Gets a multi-day weather forecast for a city.
        /// </summary>
        [HttpGet("forecast/city/{city}")]
        [ProducesResponseType(typeof(ForecastResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> GetForecastByCity(string city, [FromQuery] int days = 3, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return Task.FromResult<IActionResult>(BadRequest(BuildError("CITY_REQUIRED", "City is required.")));
            }

            // Enforce supported range for free tier
            days = Math.Clamp(days, 1, 3);

            return ExecuteAsync<ForecastResult>(
                ct => _weatherService.GetForecastByCityAsync(city, days, ct),
                $"Forecast not found for city '{city}'.",
                $"GetForecastByCity for {city} days={days}",
                cancellationToken,
                mapHttpRequestExceptionTo502: true);
        }

        /// <summary>
        /// Gets a multi-day weather forecast for specific coordinates.
        /// </summary>
        [HttpGet("forecast/coordinates")]
        [ProducesResponseType(typeof(ForecastResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ForecastResultExample))]
        public Task<IActionResult> GetForecastByCoordinates([FromQuery] double lat, [FromQuery] double lon, [FromQuery] int days = 3, CancellationToken cancellationToken = default)
        {
            days = Math.Clamp(days, 1, 3);

            return ExecuteAsync<ForecastResult>(
                ct => _weatherService.GetForecastByCoordinatesAsync(lat, lon, days, ct),
                $"Forecast not found for coordinates ({lat}, {lon}).",
                $"GetForecastByCoordinates for {lat},{lon} days={days}",
                cancellationToken,
                mapHttpRequestExceptionTo502: true);
        }

        /// <summary>
        /// Gets a combined summary of current weather, today's forecast, and astronomy data for a given city.
        /// </summary>
        [HttpGet("today-summary/{city}")]
        [ProducesResponseType(typeof(TodaySummaryResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TodaySummaryResultExample))]
        public async Task<IActionResult> GetTodaySummary(string city, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest(BuildError("CITY_REQUIRED", "City is required."));
            }

            // Use helper but keep explicit 500 mapping for clarity
            return await ExecuteAsync<TodaySummaryResult>(
                ct => _todaySummaryService.GetTodaySummaryAsync(city, ct),
                $"No summary data found for city '{city}'.",
                $"GetTodaySummary for {city}",
                cancellationToken);
        }

        /// <summary>
        /// Returns high-level weather advice for today.
        /// </summary>
        [HttpGet("advice/{city}")]
        [ProducesResponseType(typeof(WeatherAdviceResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(WeatherAdviceResultExample))]
        public Task<IActionResult> GetAdvice(string city, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return Task.FromResult<IActionResult>(BadRequest(BuildError("CITY_REQUIRED", "City is required.")));
            }

            return ExecuteAsync<WeatherAdviceResult>(
                ct => _weatherAdviceService.GetAdviceAsync(city, ct),
                $"No advice data found for city '{city}'.",
                $"GetAdvice for {city}",
                cancellationToken);
        }

        /// <summary>
        /// Returns a travel/comfort score (0–100) for today.
        /// </summary>
        [HttpGet("travel-score/{city}")]
        [ProducesResponseType(typeof(TravelScoreResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public Task<IActionResult> GetTravelScore(string city, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return Task.FromResult<IActionResult>(BadRequest(BuildError("CITY_REQUIRED", "City is required.")));
            }

            return ExecuteAsync<TravelScoreResult>(
                ct => _travelScoreService.GetTravelScoreAsync(city, ct),
                $"No travel score data found for city '{city}'.",
                $"GetTravelScore for {city}",
                cancellationToken);
        }
    }
}
