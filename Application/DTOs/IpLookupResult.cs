namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents location, time, and network information derived from an IP address lookup.
    /// </summary>
    public class IpLookupResult
    {
        /// <summary>
        /// The IP address that was resolved.
        /// </summary>
        public string Ip { get; set; } = string.Empty;

        /// <summary>
        /// City associated with the IP address.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Region or state associated with the IP address.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Country associated with the IP address.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Latitude of the inferred location.
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// Longitude of the inferred location.
        /// </summary>
        public double Lon { get; set; }

        /// <summary>
        /// IANA time zone identifier for the inferred location (e.g., "America/Los_Angeles").
        /// </summary>
        public string TimeZoneId { get; set; } = string.Empty;

        /// <summary>
        /// The local time at the inferred location as returned by the provider.
        /// Typically formatted as "yyyy-MM-dd HH:mm".
        /// </summary>
        public string LocalTime { get; set; } = string.Empty;

        /// <summary>
        /// Internet Service Provider (ISP) name, if available.
        /// </summary>
        public string Isp { get; set; } = string.Empty;
    }
}
