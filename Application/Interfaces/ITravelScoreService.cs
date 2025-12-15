using WeatherApi.Application.DTOs;

namespace WeatherApi.Application.Interfaces
{
    /// <summary>
    /// Defines operations for calculating an overall travel or comfort score
    /// for a given location, based on temperature, precipitation, UV index,
    /// and air-quality indicators.
    /// </summary>
    public interface ITravelScoreService
    {
        /// <summary>
        /// Calculates today's travel or comfort score for the specified city,
        /// combining weather and air-quality factors into a single rating.
        /// </summary>
        /// <param name="city">
        /// The city name or "City,CountryCode" identifier 
        /// (for example, "Delhi,IN") for which the score should be computed.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to observe cancellation requests for the operation.
        /// </param>
        /// <returns>
        /// A <see cref="TravelScoreResult"/> containing the computed comfort score
        /// and associated components, or <c>null</c> if no data is available 
        /// for the specified city.
        /// </returns>
        Task<TravelScoreResult?> GetTravelScoreAsync(
            string city,
            CancellationToken cancellationToken = default);
    }
}
