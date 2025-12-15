using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Infrastructure.Services
{
    /// <summary>
    /// Provides functionality to generate a combined summary of current weather,
    /// today's forecast, and astronomy information for a specified city.
    /// </summary>
    public class TodaySummaryService : ITodaySummaryService
    {
        private readonly IWeatherService _weatherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TodaySummaryService"/> class.
        /// </summary>
        /// <param name="weatherService">
        /// The underlying weather service used to retrieve current, forecast,
        /// and astronomy details.
        /// </param>
        public TodaySummaryService(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        /// Retrieves a consolidated summary of the current weather,
        /// today's forecast, and astronomy details for the specified city.
        /// </summary>
        /// <param name="city">
        /// The city name or "City,CountryCode" identifier 
        /// (for example, "Delhi,IN") for which summary information is requested.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to observe cancellation requests for the operation.
        /// </param>
        /// <returns>
        /// A <see cref="TodaySummaryResult"/> containing merged current weather,
        /// forecast, and astronomy information, or <c>null</c> if any required
        /// component could not be retrieved.
        /// </returns>
        public async Task<TodaySummaryResult?> GetTodaySummaryAsync(
            string city,
            CancellationToken cancellationToken)
        {
            var current = await _weatherService.GetCurrentByCityAsync(city, cancellationToken);
            var forecast = await _weatherService.GetForecastByCityAsync(city, 1, cancellationToken);
            var astronomy = await _weatherService.GetAstronomyAsync(city, DateTime.UtcNow, cancellationToken);

            if (current == null || forecast == null || astronomy == null)
                return null;

            return new TodaySummaryResult
            {
                Current = current,
                ForecastToday = forecast.Days.First(),
                Astronomy = astronomy
            };
        }
    }
}
