namespace WeatherApi.Infrastructure.Config
{
    /// <summary>
    /// Represents configuration settings for connecting to the external Weather API provider.
    /// These values are typically bound from <c>appsettings.json</c> using the Options pattern.
    /// </summary>
    public class WeatherApiOptions
    {
        /// <summary>
        /// The base URL of the Weather API service 
        /// (for example, "https://api.weatherapi.com/v1/").
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// The API key used to authenticate requests to the Weather API provider.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}
