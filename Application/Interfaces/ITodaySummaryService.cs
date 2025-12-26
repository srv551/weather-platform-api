using WeatherApi.Application.DTOs;

namespace WeatherApi.Application.Interfaces
{
    /// <summary>
    /// Defines operations for generating a combined daily summary
    /// of current weather, today's forecast, and astronomy information.
    /// </summary>
    public interface ITodaySummaryService
    {
        /// <summary>
        /// Retrieves a consolidated summary of current conditions,
        /// today's forecast, and astronomy details for the specified city.
        /// </summary>
        /// <param name="city">
        /// The city name or "City,CountryCode" identifier 
        /// (for example, "Delhi,IN") used to request data.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to observe cancellation requests.
        /// </param>
        /// <returns>
        /// A <see cref="TodaySummaryResult"/> object containing
        /// the merged current weather, daily forecast, and astronomy data,
        /// or <c>null</c> if no data is found for the specified location.
        /// </returns>
        Task<TodaySummaryResult?> GetTodaySummaryAsync(string city, CancellationToken cancellationToken);

        /// <summary>
        /// Generates a human-friendly daily weather insight.
        /// </summary>
        Task<DailyInsightResult?> GetDailyInsightAsync(string city, CancellationToken cancellationToken);
    }
}
