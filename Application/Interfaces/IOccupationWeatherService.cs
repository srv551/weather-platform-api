using WeatherApi.Application.DTOs;

namespace WeatherApi.Application.Interfaces
{
    /// <summary>
    /// Generates occupation-specific weather insights.
    /// </summary>
    public interface IOccupationWeatherService
    {
        /// <summary>
        /// Returns weather guidance tailored to a specific occupation.
        /// </summary>
        Task<OccupationWeatherResult?> GetOccupationWeatherAsync(
            string city,
            string occupation,
            CancellationToken cancellationToken = default);
    }
}
