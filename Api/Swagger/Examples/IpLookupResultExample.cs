using Swashbuckle.AspNetCore.Filters;
using WeatherApi.Application.DTOs;

namespace WeatherApi.Api.Swagger.Examples
{
    /// <summary>
    /// Provides example data for <see cref="IpLookupResult"/> responses.
    /// </summary>
    public class IpLookupResultExample : IExamplesProvider<IpLookupResult>
    {
        public IpLookupResult GetExamples()
        {
            return new IpLookupResult
            {
                Ip = "8.8.8.8",
                City = "Mountain View",
                Region = "California",
                Country = "United States",
                Lat = 37.4056,
                Lon = -122.0775,
                TimeZoneId = "America/Los_Angeles",

                // Fixed timestamp so example remains stable in Swagger UI
                LocalTime = "2025-12-11 14:30",

                Isp = "Google LLC"
            };
        }
    }
}
