using Swashbuckle.AspNetCore.Filters;
using WeatherApi.Application.DTOs;

namespace WeatherApi.Api.Swagger.Examples
{
    /// <summary>
    /// Provides example data for <see cref="TodaySummaryResult"/> responses.
    /// </summary>
    public class TodaySummaryResultExample : IExamplesProvider<TodaySummaryResult>
    {
        public TodaySummaryResult GetExamples()
        {
            // Fixed example date for consistent Swagger output
            var exampleDate = new DateTime(2025, 12, 11);

            return new TodaySummaryResult
            {
                Current = new WeatherResult
                {
                    City = "Delhi",
                    Region = "Delhi",
                    Country = "India",
                    LocalTime = "2025-12-11 14:35",
                    Temperature = 32.5,
                    FeelsLike = 34.0,
                    Humidity = 62,
                    PressureMb = 1002,
                    WindKph = 14.2,
                    Uv = 8,
                    Description = "Partly cloudy"
                },

                ForecastToday = new DailyForecast
                {
                    Date = exampleDate,
                    MaxTemp = 34,
                    MinTemp = 21,
                    AvgTemp = 28,
                    MaxTempF = 93.2,
                    MinTempF = 69.8,
                    AvgTempF = 82.4,
                    ChanceOfRain = 40,
                    ChanceOfSnow = 0,
                    TotalPrecipMm = 1.2,
                    TotalPrecipIn = 0.05,
                    MaxWindKph = 18,
                    MaxWindMph = 11.2,
                    AvgVisibilityKm = 10,
                    AvgVisibilityMiles = 6.2,
                    AvgHumidity = 60,
                    Uv = 7,
                    Condition = "Light rain showers"
                },

                Astronomy = new AstronomyResult
                {
                    City = "Delhi",
                    Region = "Delhi",
                    Country = "India",
                    Date = exampleDate,
                    Sunrise = "06:55 AM",
                    Sunset = "05:35 PM",
                    Moonrise = "04:40 PM",
                    Moonset = "05:12 AM",
                    MoonPhase = "Waxing Crescent",
                    MoonIllumination = 32,
                    IsMoonUp = false,
                    IsSunUp = true
                }
            };
        }
    }
}
