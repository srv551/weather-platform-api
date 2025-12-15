using Swashbuckle.AspNetCore.Filters;
using WeatherApi.Application.DTOs;

namespace WeatherApi.Api.Swagger.Examples
{
    /// <summary>
    /// Provides example data for <see cref="TimeZoneResult"/> responses.
    /// </summary>
    public class TimeZoneResultExample : IExamplesProvider<TimeZoneResult>
    {
        public TimeZoneResult GetExamples()
        {
            return new TimeZoneResult
            {
                Name = "Asia/Kolkata",
                Region = "Delhi",
                Country = "India",
                Lat = 28.6139,
                Lon = 77.2090,

                // Fixed example timestamp for stable Swagger documentation
                LocalTime = "2025-12-11 14:30",

                GmtOffsetHours = 5  // IST offset is +5:30
            };
        }
    }
}
