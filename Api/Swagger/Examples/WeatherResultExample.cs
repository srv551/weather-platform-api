using Swashbuckle.AspNetCore.Filters;
using WeatherApi.Application.DTOs;

namespace WeatherApi.Api.Swagger.Examples
{
    /// <summary>
    /// Provides example data for <see cref="WeatherResult"/> responses.
    /// </summary>
    public class WeatherResultExample : IExamplesProvider<WeatherResult>
    {
        public WeatherResult GetExamples()
        {
            return new WeatherResult
            {
                City = "Delhi",
                Region = "Delhi",
                Country = "India",

                // Fixed timestamp for consistent swagger examples
                LocalTime = "2025-12-11 14:30",

                Temperature = 32.4,
                TemperatureF = 90.3,
                FeelsLike = 35.0,
                FeelsLikeF = 95.0,

                Humidity = 62,
                PressureMb = 1005,
                PressureIn = 29.68,

                Cloud = 40,
                Uv = 7.0,

                WindKph = 12.0,
                WindMph = 7.5,
                WindDegree = 210,
                WindDirection = "SSW",

                GustKph = 20.0,
                GustMph = 12.4,

                VisibilityKm = 8.0,
                VisibilityMiles = 5.0,

                IsDay = true,
                Description = "Partly cloudy"
            };
        }
    }
}
