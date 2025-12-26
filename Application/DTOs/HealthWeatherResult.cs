namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Health-aware weather assessment for a specific condition.
    /// </summary>
    public class HealthWeatherResult
    {
        public string City { get; set; } = string.Empty;
        public string HealthCondition { get; set; } = string.Empty;

        /// <summary>
        /// Overall health risk score (0–100, higher = more risk).
        /// </summary>
        public int RiskScore { get; set; }

        /// <summary>
        /// Risk severity label.
        /// </summary>
        public string RiskLevel { get; set; } = string.Empty;

        /// <summary>
        /// Weather factors contributing to risk.
        /// </summary>
        public List<string> Triggers { get; set; } = new();

        /// <summary>
        /// Recommended precautions.
        /// </summary>
        public List<string> Recommendations { get; set; } = new();

        /// <summary>
        /// Best time window for outdoor activity, if any.
        /// </summary>
        public string SafeTimeWindow { get; set; } = string.Empty;
    }
}
