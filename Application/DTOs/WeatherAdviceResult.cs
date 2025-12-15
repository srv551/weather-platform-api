namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents high-level weather advice for a specific location for today.
    /// Consolidates rain, heat, UV, and air-quality indicators into simple,
    /// user-friendly recommendations.
    /// </summary>
    public class WeatherAdviceResult
    {
        /// <summary>
        /// Indicates whether the user should carry an umbrella today.
        /// </summary>
        public bool ShouldCarryUmbrella { get; set; }

        /// <summary>
        /// A descriptive explanation supporting the umbrella recommendation.
        /// </summary>
        public string UmbrellaReason { get; set; } = string.Empty;

        /// <summary>
        /// Expected chance of rain for today, expressed as a percentage.
        /// </summary>
        public int ChanceOfRain { get; set; }

        /// <summary>
        /// Expected total precipitation for the day, expressed in millimeters.
        /// </summary>
        public double TotalPrecipMm { get; set; }

        /// <summary>
        /// Indicates whether heat-related precautions are advised
        /// (for example, hydration or avoiding peak afternoon exposure).
        /// </summary>
        public bool HeatWarning { get; set; }

        /// <summary>
        /// Indicates whether strong UV protection measures (such as sunscreen)
        /// are recommended today.
        /// </summary>
        public bool UvWarning { get; set; }

        /// <summary>
        /// Indicates whether air quality is poor enough to recommend caution
        /// (for example, limiting outdoor activity for sensitive groups).
        /// </summary>
        public bool AirQualityWarning { get; set; }

        /// <summary>
        /// Optional Air Quality Index (US EPA standard), if available.
        /// A higher value indicates poorer air quality.
        /// </summary>
        public int? AirQualityIndex { get; set; }

        /// <summary>
        /// Optional descriptive notes about air quality conditions.
        /// </summary>
        public string AirQualityNotes { get; set; } = string.Empty;

        /// <summary>
        /// Optional general suggestions or guidance for the user,
        /// derived from the overall weather conditions.
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
}
