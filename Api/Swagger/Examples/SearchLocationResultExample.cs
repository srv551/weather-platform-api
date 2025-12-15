using Swashbuckle.AspNetCore.Filters;
using WeatherApi.Application.DTOs;

namespace WeatherApi.Api.Swagger.Examples
{
    /// <summary>
    /// Provides example data for <see cref="SearchLocationResult"/> responses.
    /// </summary>
    public class SearchLocationResultExample : IExamplesProvider<List<SearchLocationResult>>
    {
        public List<SearchLocationResult> GetExamples()
        {
            return new List<SearchLocationResult>
            {
                new SearchLocationResult
                {
                    Id = 1,
                    Name = "London",
                    Region = "City of London, Greater London",
                    Country = "United Kingdom",
                    Lat = 51.52,
                    Lon = -0.11,
                    Url = "United-Kingdom/England/London"
                },
                new SearchLocationResult
                {
                    Id = 2,
                    Name = "London",
                    Region = "Ontario",
                    Country = "Canada",
                    Lat = 42.98,
                    Lon = -81.25,
                    Url = "Canada/Ontario/London"
                }
            };
        }
    }
}
