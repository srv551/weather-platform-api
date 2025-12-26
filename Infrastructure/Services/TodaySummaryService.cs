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

        public async Task<DailyInsightResult?> GetDailyInsightAsync(
            string city,
            CancellationToken cancellationToken)
        {
            var summary = await GetTodaySummaryAsync(city, cancellationToken);
            if (summary == null) return null;

            var current = summary.Current;
            var forecast = summary.ForecastToday;

            var reasons = new List<string>();

            if (current.FeelsLike >= 35)
                reasons.Add($"Feels like {current.FeelsLike:0}°C");

            if (forecast.ChanceOfRain >= 50)
                reasons.Add($"Rain chance {forecast.ChanceOfRain}%");

            if (current.Uv >= 8)
                reasons.Add($"High UV index ({current.Uv})");

            if (current.AirQuality?.UsEpaIndex >= 4)
                reasons.Add("Poor air quality");

            var summaryText =
                current.FeelsLike >= 35
                    ? "Hot and potentially uncomfortable day."
                    : forecast.ChanceOfRain >= 60
                        ? "Rainy conditions likely today."
                        : "Generally comfortable weather conditions.";

            var (bestTime, worstTime) =
            InsightTimeWindowCalculator.Calculate(current, forecast, summary.Astronomy);

            return new DailyInsightResult
            {
                City = current.City,
                Summary = summaryText,
                Confidence = "High",
                Reasons = reasons,
                BestOutdoorTime = bestTime,
                WorstOutdoorTime = worstTime,
                Recommendation =
                    reasons.Any()
                        ? "Plan outdoor activities carefully and take precautions."
                        : "A good day for outdoor activities."
            };
        }
    }
}
