using WeatherApi.Application.DTOs;

namespace WeatherApi.Application.Interfaces
{
    /// <summary>
    /// Defines operations for retrieving current weather, forecasts, time zone details,
    /// astronomy data, IP-based location information, and search/autocomplete results
    /// from the underlying weather provider.
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Retrieves the current weather conditions for the specified city.
        /// </summary>
        /// <param name="city">
        /// The city name or "City,CountryCode" identifier 
        /// (for example, "Delhi,IN").
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to observe cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="WeatherResult"/> containing current weather conditions,
        /// or <c>null</c> if no data is available for the specified location.
        /// </returns>
        Task<WeatherResult?> GetCurrentByCityAsync(
            string city,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current weather conditions for the specified coordinates.
        /// </summary>
        /// <param name="latitude">The latitude of the location in decimal degrees.</param>
        /// <param name="longitude">The longitude of the location in decimal degrees.</param>
        /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
        /// <returns>
        /// A <see cref="WeatherResult"/> containing current weather details,
        /// or <c>null</c> if no data is found for the given coordinates.
        /// </returns>
        Task<WeatherResult?> GetCurrentByCoordinatesAsync(
            double latitude,
            double longitude,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a multi-day weather forecast for the specified city.
        /// </summary>
        /// <param name="city">
        /// The city name or "City,CountryCode" identifier 
        /// (for example, "Delhi,IN").
        /// </param>
        /// <param name="days">
        /// Number of days to retrieve in the forecast (provider limits typically apply).
        /// </param>
        /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
        /// <returns>
        /// A <see cref="ForecastResult"/> containing forecast details,
        /// or <c>null</c> if forecast data is unavailable.
        /// </returns>
        Task<ForecastResult?> GetForecastByCityAsync(
            string city,
            int days,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a multi-day weather forecast for the specified coordinates.
        /// </summary>
        /// <param name="latitude">The latitude of the location in decimal degrees.</param>
        /// <param name="longitude">The longitude of the location in decimal degrees.</param>
        /// <param name="days">
        /// Number of days to retrieve in the forecast (provider limits typically apply).
        /// </param>
        /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
        /// <returns>
        /// A <see cref="ForecastResult"/> containing forecast data,
        /// or <c>null</c> if no forecast is available for the coordinates.
        /// </returns>
        Task<ForecastResult?> GetForecastByCoordinatesAsync(
            double latitude,
            double longitude,
            int days,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves time zone information for the specified location.
        /// </summary>
        /// <param name="query">
        /// A location query such as a city name or coordinates 
        /// (for example, "Delhi" or "51.52,-0.11").
        /// </param>
        /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
        /// <returns>
        /// A <see cref="TimeZoneResult"/> containing time zone details,
        /// or <c>null</c> if the provider returns no data.
        /// </returns>
        Task<TimeZoneResult?> GetTimeZoneAsync(
            string query,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves astronomy information (sunrise, sunset, moon details, etc.)
        /// for the specified location and date.
        /// </summary>
        /// <param name="query">
        /// A location query such as a city name or coordinates 
        /// (for example, "Delhi" or "51.52,-0.11").
        /// </param>
        /// <param name="date">The date for which astronomy details are requested.</param>
        /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
        /// <returns>
        /// An <see cref="AstronomyResult"/> containing sun and moon details,
        /// or <c>null</c> if no astronomy data is available.
        /// </returns>
        Task<AstronomyResult?> GetAstronomyAsync(
            string query,
            DateTime date,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs an IP lookup to determine approximate location and time zone information.
        /// </summary>
        /// <param name="ip">
        /// IP address to query. Use "auto:ip" to let the provider detect caller IP.
        /// </param>
        /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
        /// <returns>
        /// An <see cref="IpLookupResult"/> describing the location associated with the IP address,
        /// or <c>null</c> if the provider cannot resolve it.
        /// </returns>
        Task<IpLookupResult?> GetIpLookupAsync(
            string ip,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches for possible matching locations for the specified text,
        /// returning autocomplete-like suggestions.
        /// </summary>
        /// <param name="query">A search term such as "lon" or "London".</param>
        /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
        /// <returns>
        /// A list of <see cref="SearchLocationResult"/> entries that match the query.
        /// An empty list is returned if no matches exist.
        /// </returns>
        Task<List<SearchLocationResult>> SearchLocationsAsync(
            string query,
            CancellationToken cancellationToken = default);
    }
}
