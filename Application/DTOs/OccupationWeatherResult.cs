namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Advanced occupation-specific weather intelligence.
    /// </summary>
    public class OccupationWeatherResult
    {
        public string City { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;

        /// <summary>
        /// Overall suitability score (0–100).
        /// </summary>
        public int SuitabilityScore { get; set; }

        public string SuitabilityLabel { get; set; } = string.Empty;

        /// <summary>
        /// What works in favour today.
        /// </summary>
        public List<string> Opportunities { get; set; } = new();

        /// <summary>
        /// What can cause disruption or harm.
        /// </summary>
        public List<string> Risks { get; set; } = new();

        /// <summary>
        /// Clear recommended actions.
        /// </summary>
        public List<string> RecommendedActions { get; set; } = new();

        /// <summary>
        /// Time windows where conditions are optimal.
        /// </summary>
        public string BestTimeWindow { get; set; } = string.Empty;
    }
}
