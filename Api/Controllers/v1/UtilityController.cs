using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using WeatherApi.Api.Swagger.Examples;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Api.Controllers.V1
{
    /// <summary>
    /// Provides utility endpoints for time zone, astronomy, IP lookup, and location search.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UtilityController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<UtilityController> _logger;

        public UtilityController(IWeatherService weatherService, ILogger<UtilityController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        #region Helpers

        private IActionResult BuildNotFound(string code, string message)
        {
            return NotFound(new ErrorResponse
            {
                Code = code,
                Message = message,
                TraceId = HttpContext.TraceIdentifier
            });
        }

        private IActionResult BuildUnexpectedError()
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                Code = "UNEXPECTED_ERROR",
                Message = "An unexpected error occurred.",
                TraceId = HttpContext.TraceIdentifier
            });
        }

        private async Task<IActionResult> ExecuteAsync<T>(
            Func<CancellationToken, Task<T?>> operation,
            string notFoundMessage,
            string logMessageContext,
            CancellationToken cancellationToken) where T : class
        {
            try
            {
                var result = await operation(cancellationToken);

                if (result == null)
                {
                    _logger.LogInformation("NotFound in {Context}: {Message}", logMessageContext, notFoundMessage);
                    return BuildNotFound("NOT_FOUND", notFoundMessage);
                }

                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Operation cancelled in {Context}", logMessageContext);
                return StatusCode(StatusCodes.Status499ClientClosedRequest); // or appropriate response
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Context}", logMessageContext);
                return BuildUnexpectedError();
            }
        }

        #endregion

        /// <summary>
        /// Gets time zone information for a given city or set of coordinates.
        /// </summary>
        [HttpGet("timezone")]
        [ProducesResponseType(typeof(TimeZoneResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TimeZoneResultExample))]
        public Task<IActionResult> GetTimeZone(
            [Required][FromQuery] string locationQuery,
            CancellationToken cancellationToken)
        {
            string notFoundMessage = $"Time zone not found for '{locationQuery}'.";
            return ExecuteAsync<TimeZoneResult>(
                ct => _weatherService.GetTimeZoneAsync(locationQuery, ct),
                notFoundMessage,
                $"GetTimeZone for {locationQuery}",
                cancellationToken);
        }

        /// <summary>
        /// Gets astronomy information (sunrise, sunset, moon) for a given location and date.
        /// </summary>
        [HttpGet("astronomy")]
        [ProducesResponseType(typeof(AstronomyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AstronomyResultExample))]
        public Task<IActionResult> GetAstronomy(
            [Required][FromQuery] string locationQuery,
            [FromQuery] DateTime? date,
            CancellationToken cancellationToken)
        {
            var targetDate = date ?? DateTime.UtcNow.Date;
            string notFoundMessage = $"Astronomy data not found for '{locationQuery}' on {targetDate:yyyy-MM-dd}.";

            return ExecuteAsync<AstronomyResult>(
                ct => _weatherService.GetAstronomyAsync(locationQuery, targetDate, ct),
                notFoundMessage,
                $"GetAstronomy for {locationQuery} on {targetDate:yyyy-MM-dd}",
                cancellationToken);
        }

        /// <summary>
        /// Looks up location and time zone information for an IP address.
        /// </summary>
        [HttpGet("iplookup")]
        [ProducesResponseType(typeof(IpLookupResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(IpLookupResultExample))]
        public Task<IActionResult> GetIpLookup(
            [FromQuery] string? ipAddress,
            CancellationToken cancellationToken)
        {
            var queryIp = string.IsNullOrWhiteSpace(ipAddress) ? "auto:ip" : ipAddress;
            string notFoundMessage = $"IP lookup failed for '{queryIp}'.";

            return ExecuteAsync<IpLookupResult>(
                ct => _weatherService.GetIpLookupAsync(queryIp, ct),
                notFoundMessage,
                $"GetIpLookup for {queryIp}",
                cancellationToken);
        }

        /// <summary>
        /// Searches for locations that match the specified text.
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<SearchLocationResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchLocationResultExample))]
        public async Task<IActionResult> SearchLocations(
            [Required][FromQuery] string searchText,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _weatherService.SearchLocationsAsync(searchText, cancellationToken);
                // For search, an empty list is a valid response so we return Ok(result) directly.
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("SearchLocations cancelled for {SearchText}", searchText);
                return StatusCode(StatusCodes.Status499ClientClosedRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchLocations for searchText {SearchText}", searchText);
                return BuildUnexpectedError();
            }
        }
    }
}
