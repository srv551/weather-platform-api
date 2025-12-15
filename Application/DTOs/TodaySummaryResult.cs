namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents a consolidated summary of today's weather conditions,
    /// including the current weather, today's forecast, and astronomy details
    /// (sunrise, sunset, moon phase, etc.).
    /// </summary>
    public class TodaySummaryResult
    {
        /// <summary>
        /// Current weather conditions for the specified location.
        /// Includes temperature, humidity, wind, visibility, and general description.
        /// </summary>
        public WeatherResult Current { get; set; } = new();

        /// <summary>
        /// Forecast data specifically for today.
        /// Includes temperatures, precipitation metrics, UV index, and expected conditions.
        /// </summary>
        public DailyForecast ForecastToday { get; set; } = new();

        /// <summary>
        /// Astronomy information for today, including sunrise/sunset,
        /// moonrise/moonset, and moon illumination details.
        /// </summary>
        public AstronomyResult Astronomy { get; set; } = new();
    }
}
