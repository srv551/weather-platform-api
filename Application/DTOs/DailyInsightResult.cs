namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Human-friendly daily weather insight derived from weather, forecast, AQI, and astronomy data.
    /// </summary>
    public class DailyInsightResult
    {
        /// <summary>
        /// City for which the insight applies.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// One-line human-readable weather summary.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Confidence level of the insight (Low / Medium / High).
        /// </summary>
        public string Confidence { get; set; } = "High";

        /// <summary>
        /// Key reasons behind the summary.
        /// </summary>
        public List<string> Reasons { get; set; } = new();

        /// <summary>
        /// Best time window for outdoor activities.
        /// </summary>
        public string BestOutdoorTime { get; set; } = string.Empty;

        /// <summary>
        /// Worst time window for outdoor activities.
        /// </summary>
        public string WorstOutdoorTime { get; set; } = string.Empty;

        /// <summary>
        /// Clear recommendation for the user.
        /// </summary>
        public string Recommendation { get; set; } = string.Empty;
    }
}
