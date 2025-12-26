using WeatherApi.Application.DTOs;
using WeatherApi.Application.Domain;

namespace WeatherApi.Application.Interfaces
{
    /// <summary>
    /// Provides health-aware weather insights based on current conditions.
    /// </summary>
    public interface IHealthWeatherService
    {
        Task<HealthWeatherResult?> GetHealthInsightAsync(
            string city,
            HealthConditionType condition,
            CancellationToken cancellationToken = default);
    }
}
