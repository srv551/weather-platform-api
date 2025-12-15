namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents astronomy information (sun and moon) for a specific location and date.
    /// </summary>
    public class AstronomyResult
    {
        /// <summary>
        /// City name returned by the provider.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Administrative region of the location.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Country name of the location.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Date for which astronomy data is provided.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Local sunrise time (provider-formatted string).
        /// </summary>
        public string Sunrise { get; set; } = string.Empty;

        /// <summary>
        /// Local sunset time (provider-formatted string).
        /// </summary>
        public string Sunset { get; set; } = string.Empty;

        /// <summary>
        /// Local moonrise time (provider-formatted string).
        /// </summary>
        public string Moonrise { get; set; } = string.Empty;

        /// <summary>
        /// Local moonset time (provider-formatted string).
        /// </summary>
        public string Moonset { get; set; } = string.Empty;

        /// <summary>
        /// The current moon phase (e.g., “Waxing Crescent”, “Full Moon”).
        /// </summary>
        public string MoonPhase { get; set; } = string.Empty;

        /// <summary>
        /// Illuminated percentage of the moon’s visible disk.
        /// </summary>
        public int MoonIllumination { get; set; }

        /// <summary>
        /// True if the moon is above the horizon at the time of the data snapshot.
        /// </summary>
        public bool IsMoonUp { get; set; }

        /// <summary>
        /// True if the sun is above the horizon at the time of the data snapshot.
        /// </summary>
        public bool IsSunUp { get; set; }
    }
}
