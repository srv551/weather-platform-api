using WeatherApi.Application.DTOs;

namespace WeatherApi.Application.Interfaces
{
    /// <summary>
    /// Defines operations for generating high-level weather advice for a given location,
    /// including recommendations related to rain, UV exposure, heat, and air quality.
    /// </summary>
    public interface IWeatherAdviceService
    {
        /// <summary>
        /// Computes weather-based advice for the specified city by analysing 
        /// current conditions and today's forecast.
        /// </summary>
        /// <param name="city">
        /// The city name or "City,CountryCode" identifier 
        /// (for example, "Delhi,IN") for which advice should be generated.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to observe cancellation requests for the operation.
        /// </param>
        /// <returns>
        /// A <see cref="WeatherAdviceResult"/> containing umbrella, heat, UV, 
        /// and air-quality recommendations, or <c>null</c> if data could not be retrieved.
        /// </returns>
        Task<WeatherAdviceResult?> GetAdviceAsync(
            string city,
            CancellationToken cancellationToken = default);
    }
}
