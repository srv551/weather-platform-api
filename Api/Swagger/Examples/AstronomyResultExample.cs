using Swashbuckle.AspNetCore.Filters;
using WeatherApi.Application.DTOs;

namespace WeatherApi.Api.Swagger.Examples
{
    /// <summary>
    /// Provides example data for <see cref="AstronomyResult"/> responses.
    /// </summary>
    public class AstronomyResultExample : IExamplesProvider<AstronomyResult>
    {
        public AstronomyResult GetExamples()
        {
            // Use a fixed date so the example remains stable across docs/snapshots.
            var exampleDate = new DateTime(2025, 12, 11);

            return new AstronomyResult
            {
                City = "Delhi",
                Region = "Delhi",
                Country = "India",
                Date = exampleDate,
                Sunrise = "06:45 AM",
                Sunset = "05:30 PM",
                Moonrise = "07:10 PM",
                Moonset = "06:30 AM",
                MoonPhase = "Waxing gibbous",
                MoonIllumination = 80,
                IsMoonUp = true,
                IsSunUp = false
            };
        }
    }
}
