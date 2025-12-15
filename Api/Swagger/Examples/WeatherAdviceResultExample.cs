using Swashbuckle.AspNetCore.Filters;
using WeatherApi.Application.DTOs;

namespace WeatherApi.Api.Swagger.Examples
{
    /// <summary>
    /// Provides example data for <see cref="WeatherAdviceResult"/> responses.
    /// </summary>
    public class WeatherAdviceResultExample : IExamplesProvider<WeatherAdviceResult>
    {
        public WeatherAdviceResult GetExamples()
        {
            return new WeatherAdviceResult
            {
                ShouldCarryUmbrella = true,
                UmbrellaReason = "Chance of rain is 70% with approximately 3.2 mm precipitation expected.",
                ChanceOfRain = 70,
                TotalPrecipMm = 3.2,

                HeatWarning = true,
                UvWarning = true,

                Notes =
                    "Expect warm and humid conditions. Stay hydrated, avoid peak afternoon heat, " +
                    "use sunscreen, and wear sunglasses if going outdoors."
            };
        }
    }
}
