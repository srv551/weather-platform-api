namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents time zone information for a specific geographic location.
    /// </summary>
    public class TimeZoneResult
    {
        /// <summary>
        /// IANA time zone identifier (for example, "Asia/Kolkata").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Administrative region for the location (state, province, or district).
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Country name for the location.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Latitude of the location in decimal degrees.
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Longitude of the location in decimal degrees.
        /// </summary>
        public double Lon { get; set; }

        /// <summary>
        /// Local date and time at the location, formatted as returned by the provider 
        /// (typically "yyyy-MM-dd HH:mm").
        /// </summary>
        public string LocalTime { get; set; } = string.Empty;

        /// <summary>
        /// GMT/UTC offset in hours (e.g., 5 for IST, -8 for PST).
        /// </summary>
        public int GmtOffsetHours { get; set; }
    }
}
