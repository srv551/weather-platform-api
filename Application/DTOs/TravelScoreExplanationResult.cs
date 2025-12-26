namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Detailed, explainable breakdown of the daily travel score.
    /// </summary>
    public class TravelScoreExplanationResult
    {
        /// <summary>
        /// City for which the explanation applies.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Overall travel score (0–100).
        /// </summary>
        public int OverallScore { get; set; }

        /// <summary>
        /// Comfort label derived from the overall score.
        /// </summary>
        public string ComfortLabel { get; set; } = string.Empty;

        /// <summary>
        /// Confidence in the calculated score.
        /// </summary>
        public string Confidence { get; set; } = "High";

        /// <summary>
        /// Breakdown explaining how each factor contributed.
        /// </summary>
        public Dictionary<string, string> Breakdown { get; set; } = new();

        /// <summary>
        /// Human-readable explanation summary.
        /// </summary>
        public string Explanation { get; set; } = string.Empty;
    }
}
