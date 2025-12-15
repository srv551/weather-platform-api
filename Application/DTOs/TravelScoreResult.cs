namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents an overall travel and comfort score for a location for today.
    /// This score is derived by combining temperature, precipitation,
    /// UV index, and air-quality into a unified rating from 0 to 100.
    /// </summary>
    public class TravelScoreResult
    {
        /// <summary>
        /// Name of the city for which the travel score was calculated.
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
        /// Overall travel / comfort score from 0 to 100.
        /// A higher score indicates more favorable travel conditions.
        /// </summary>
        public int OverallScore { get; set; }

        /// <summary>
        /// Human-readable label derived from <see cref="OverallScore"/>
        /// such as “Poor”, “Fair”, “Good”, or “Excellent”.
        /// </summary>
        public string ComfortLabel { get; set; } = string.Empty;

        /// <summary>
        /// Optional short description explaining why the score was assigned.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Contribution of temperature to the total score (0–25).
        /// </summary>
        public int TemperatureScore { get; set; }

        /// <summary>
        /// Contribution of precipitation / rain likelihood to the total score (0–25).
        /// </summary>
        public int RainScore { get; set; }

        /// <summary>
        /// Contribution of UV index to the total score (0–25).
        /// </summary>
        public int UvScore { get; set; }

        /// <summary>
        /// Contribution of air quality to the total score (0–25).
        /// </summary>
        public int AirQualityScore { get; set; }

        /// <summary>
        /// List of warnings relevant for travellers (e.g., “High UV”, “Poor Air Quality”).
        /// May be empty if no significant risks are present.
        /// </summary>
        public List<string> Warnings { get; set; } = new();
    }
}
