namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents a single day's forecast.
    /// </summary>
    public class DailyForecast
    {
        /// <summary>
        /// Date for which this forecast applies (local date at the location).
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Maximum temperature in degrees Celsius.
        /// </summary>
        public double MaxTemp { get; set; }

        /// <summary>
        /// Minimum temperature in degrees Celsius.
        /// </summary>
        public double MinTemp { get; set; }

        /// <summary>
        /// Average temperature in degrees Celsius.
        /// </summary>
        public double AvgTemp { get; set; }

        /// <summary>
        /// Maximum temperature in degrees Fahrenheit.
        /// </summary>
        public double MaxTempF { get; set; }

        /// <summary>
        /// Minimum temperature in degrees Fahrenheit.
        /// </summary>
        public double MinTempF { get; set; }

        /// <summary>
        /// Average temperature in degrees Fahrenheit.
        /// </summary>
        public double AvgTempF { get; set; }

        /// <summary>
        /// Total precipitation for the day in millimeters.
        /// </summary>
        public double TotalPrecipMm { get; set; }

        /// <summary>
        /// Total precipitation for the day in inches.
        /// </summary>
        public double TotalPrecipIn { get; set; }

        /// <summary>
        /// Chance of rain as a percentage (0–100).
        /// </summary>
        public int ChanceOfRain { get; set; }

        /// <summary>
        /// Chance of snow as a percentage (0–100).
        /// </summary>
        public int ChanceOfSnow { get; set; }

        /// <summary>
        /// Maximum wind speed in kilometers per hour.
        /// </summary>
        public double MaxWindKph { get; set; }

        /// <summary>
        /// Maximum wind speed in miles per hour.
        /// </summary>
        public double MaxWindMph { get; set; }

        /// <summary>
        /// UV index forecast for the day (numeric index).
        /// </summary>
        public double Uv { get; set; }

        /// <summary>
        /// Average visibility in kilometers.
        /// </summary>
        public double AvgVisibilityKm { get; set; }

        /// <summary>
        /// Average visibility in miles.
        /// </summary>
        public double AvgVisibilityMiles { get; set; }

        /// <summary>
        /// Average humidity as a percentage.
        /// </summary>
        public double AvgHumidity { get; set; }

        /// <summary>
        /// Text description of the expected conditions (for example, "Light rain").
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Optional air-quality metrics for the day (may be null if provider omits).
        /// </summary>
        public AirQualityResult? AirQuality { get; set; }
    }

    /// <summary>
    /// Represents a multi-day forecast for a specific location.
    /// </summary>
    public class ForecastResult
    {
        /// <summary>
        /// Name of the city returned by the weather provider.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Administrative region for the location.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Country name for the location.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// List of daily forecast entries.
        /// </summary>
        public List<DailyForecast> Days { get; set; } = new();
    }
}
