namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents the current weather conditions for a specific location.
    /// Captures temperature, humidity, wind, pressure, visibility, and optional air-quality data.
    /// </summary>
    public class WeatherResult
    {
        /// <summary>
        /// Name of the city returned by the weather provider.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Administrative region for the location (for example, state or province).
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Country name for the location.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Local date and time at the location, as provided by the weather API.
        /// </summary>
        public string LocalTime { get; set; } = string.Empty;

        /// <summary>
        /// Current air temperature in degrees Celsius.
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Current air temperature in degrees Fahrenheit.
        /// </summary>
        public double TemperatureF { get; set; }

        /// <summary>
        /// What the temperature feels like to humans in degrees Celsius.
        /// </summary>
        public double FeelsLike { get; set; }

        /// <summary>
        /// What the temperature feels like to humans in degrees Fahrenheit.
        /// </summary>
        public double FeelsLikeF { get; set; }

        /// <summary>
        /// Relative humidity as a percentage.
        /// </summary>
        public int Humidity { get; set; }

        /// <summary>
        /// Atmospheric pressure in millibars.
        /// </summary>
        public double PressureMb { get; set; }

        /// <summary>
        /// Atmospheric pressure in inches of mercury.
        /// </summary>
        public double PressureIn { get; set; }

        /// <summary>
        /// Total cloud cover as a percentage.
        /// </summary>
        public int Cloud { get; set; }

        /// <summary>
        /// UV index at the location.
        /// </summary>
        public double Uv { get; set; }

        /// <summary>
        /// Wind speed in kilometers per hour.
        /// </summary>
        public double WindKph { get; set; }

        /// <summary>
        /// Wind speed in miles per hour.
        /// </summary>
        public double WindMph { get; set; }

        /// <summary>
        /// Wind direction in degrees, where 0°/360° indicates north.
        /// </summary>
        public int WindDegree { get; set; }

        /// <summary>
        /// Wind direction as a compass point (for example, N, NE, SW).
        /// </summary>
        public string WindDirection { get; set; } = string.Empty;

        /// <summary>
        /// Maximum expected wind gust in kilometers per hour.
        /// </summary>
        public double GustKph { get; set; }

        /// <summary>
        /// Maximum expected wind gust in miles per hour.
        /// </summary>
        public double GustMph { get; set; }

        /// <summary>
        /// Visibility in kilometers.
        /// </summary>
        public double VisibilityKm { get; set; }

        /// <summary>
        /// Visibility in miles.
        /// </summary>
        public double VisibilityMiles { get; set; }

        /// <summary>
        /// Indicates whether it is daytime (true) or nighttime (false) at the location.
        /// </summary>
        public bool IsDay { get; set; }

        /// <summary>
        /// Text description of the current weather (for example, "Partly cloudy").
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Optional air-quality details for the location, when provided by the weather API.
        /// May be null if the provider does not include air-quality data.
        /// </summary>
        public AirQualityResult? AirQuality { get; set; }
    }
}
