using Swashbuckle.AspNetCore.Filters;
using WeatherApi.Application.DTOs;

namespace WeatherApi.Api.Swagger.Examples
{
    /// <summary>
    /// Provides example data for <see cref="ForecastResult"/> responses.
    /// </summary>
    public class ForecastResultExample : IExamplesProvider<ForecastResult>
    {
        public ForecastResult GetExamples()
        {
            // Use a fixed date so Swagger examples remain stable across deployments.
            var date = new DateTime(2025, 12, 11);

            return new ForecastResult
            {
                City = "Delhi",
                Region = "Delhi",
                Country = "India",
                Days = new List<DailyForecast>
                {
                    new DailyForecast
                    {
                        Date = date,
                        MaxTemp = 34,
                        MinTemp = 26,
                        AvgTemp = 30,
                        MaxTempF = 93.2,
                        MinTempF = 78.8,
                        AvgTempF = 86.0,
                        TotalPrecipMm = 1.2,
                        TotalPrecipIn = 0.05,
                        ChanceOfRain = 40,
                        ChanceOfSnow = 0,
                        MaxWindKph = 18,
                        MaxWindMph = 11.2,
                        Uv = 8,
                        AvgVisibilityKm = 10,
                        AvgVisibilityMiles = 6.2,
                        AvgHumidity = 60,
                        Condition = "Partly cloudy"
                    }
                }
            };
        }
    }
}
