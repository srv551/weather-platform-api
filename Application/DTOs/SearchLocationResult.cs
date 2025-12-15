namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents a location returned from the search/autocomplete API.
    /// </summary>
    public class SearchLocationResult
    {
        /// <summary>
        /// Provider-specific identifier for the location.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Display name of the location (for example, "London").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Administrative region (for example, "Greater London", "Ontario").
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Country name of the location.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Latitude in decimal degrees.
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Longitude in decimal degrees.
        /// </summary>
        public double Lon { get; set; }

        /// <summary>
        /// Relative URL representing the provider's internal route to this location.
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}
