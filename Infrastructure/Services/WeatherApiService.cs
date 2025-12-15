using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;
using WeatherApi.Infrastructure.Config;
using WeatherApi.Infrastructure.External.WeatherApi;

namespace WeatherApi.Infrastructure.Services
{
    /// <summary>
    /// Default implementation of <see cref="IWeatherService"/> responsible for
    /// communicating with WeatherAPI.com, mapping raw provider responses into
    /// internal DTOs, and applying in-memory caching for performance.
    /// </summary>
    public class WeatherApiService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiOptions _options;
        private readonly IMemoryCache _cache;

        // Cache durations
        private static readonly TimeSpan CurrentCacheDuration = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan ForecastCacheDuration = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherApiService"/> class.
        /// </summary>
        /// <param name="httpClient">Configured <see cref="HttpClient"/> used for calling WeatherAPI.com.</param>
        /// <param name="options">Typed configuration containing WeatherAPI base URL and API key.</param>
        /// <param name="cache">In-memory cache used to store current and forecast data.</param>
        public WeatherApiService(
            HttpClient httpClient,
            IOptions<WeatherApiOptions> options,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _cache = cache;

            if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            {
                _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            }
        }

        // ---------------------------------------------------------------------------------------------
        // CURRENT WEATHER (CITY)
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Retrieves current weather conditions for the specified city, using caching
        /// to minimise unnecessary external API calls.
        /// </summary>
        /// <param name="city">City name or "City,CountryCode" (e.g. "Delhi,IN").</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A <see cref="WeatherResult"/> or <c>null</c> if not found.</returns>
        public async Task<WeatherResult?> GetCurrentByCityAsync(string city, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.", nameof(city));

            var cacheKey = $"current:city:{city.ToLowerInvariant()}";

            if (_cache.TryGetValue(cacheKey, out WeatherResult cached))
            {
                return cached;
            }

            var url = $"current.json?key={_options.ApiKey}&q={Uri.EscapeDataString(city)}&aqi=yes";
            var result = await GetCurrentAsync(url, cancellationToken);

            if (result != null)
            {
                _cache.Set(cacheKey, result, CurrentCacheDuration);
            }

            return result;
        }

        // ---------------------------------------------------------------------------------------------
        // CURRENT WEATHER (COORDINATES)
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Retrieves current weather conditions using geographic coordinates.
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees.</param>
        /// <param name="longitude">Longitude in decimal degrees.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A <see cref="WeatherResult"/> or <c>null</c> if not found.</returns>
        public async Task<WeatherResult?> GetCurrentByCoordinatesAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            var location = FormatCoordinates(latitude, longitude);
            var cacheKey = $"current:coords:{location}";

            if (_cache.TryGetValue(cacheKey, out WeatherResult cached))
            {
                return cached;
            }

            var url = $"current.json?key={_options.ApiKey}&q={location}&aqi=yes";
            var result = await GetCurrentAsync(url, cancellationToken);

            if (result != null)
            {
                _cache.Set(cacheKey, result, CurrentCacheDuration);
            }

            return result;
        }

        // ---------------------------------------------------------------------------------------------
        // FORECAST (CITY)
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Retrieves a multi-day weather forecast for the specified city.
        /// </summary>
        /// <param name="city">City name or "City,CountryCode".</param>
        /// <param name="days">Number of forecast days requested.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A <see cref="ForecastResult"/> or <c>null</c> if not found.</returns>
        public async Task<ForecastResult?> GetForecastByCityAsync(string city, int days, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.", nameof(city));

            var cacheKey = $"forecast:city:{city.ToLowerInvariant()}:days:{days}";

            if (_cache.TryGetValue(cacheKey, out ForecastResult cached))
            {
                return cached;
            }

            var url = $"forecast.json?key={_options.ApiKey}&q={Uri.EscapeDataString(city)}&days={days}&aqi=yes&alerts=no";
            var result = await GetForecastAsync(url, cancellationToken);

            if (result != null)
            {
                _cache.Set(cacheKey, result, ForecastCacheDuration);
            }

            return result;
        }

        // ---------------------------------------------------------------------------------------------
        // FORECAST (COORDINATES)
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Retrieves a multi-day weather forecast for the specified geographic coordinates.
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees.</param>
        /// <param name="longitude">Longitude in decimal degrees.</param>
        /// <param name="days">Number of forecast days requested.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A <see cref="ForecastResult"/> or <c>null</c>.</returns>
        public async Task<ForecastResult?> GetForecastByCoordinatesAsync(double latitude, double longitude, int days, CancellationToken cancellationToken = default)
        {
            var location = FormatCoordinates(latitude, longitude);
            var cacheKey = $"forecast:coords:{location}:days:{days}";

            if (_cache.TryGetValue(cacheKey, out ForecastResult cached))
            {
                return cached;
            }

            var url = $"forecast.json?key={_options.ApiKey}&q={location}&days={days}&aqi=yes&alerts=no";
            var result = await GetForecastAsync(url, cancellationToken);

            if (result != null)
            {
                _cache.Set(cacheKey, result, ForecastCacheDuration);
            }

            return result;
        }

        // =============================================================================================
        // PRIVATE HELPERS
        // =============================================================================================

        /// <summary>
        /// Formats latitude/longitude values into the string format required by WeatherAPI.
        /// </summary>
        private static string FormatCoordinates(double latitude, double longitude)
        {
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);
            return $"{lat},{lon}";
        }

        /// <summary>
        /// Sends a request to the WeatherAPI current weather endpoint and maps the response.
        /// </summary>
        private async Task<WeatherResult?> GetCurrentAsync(string url, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<WeatherApiCurrentResponse>(cancellationToken: cancellationToken);

            if (apiResponse == null)
                return null;

            var cur = apiResponse.Current;
            var loc = apiResponse.Location;

            return new WeatherResult
            {
                City = loc?.Name ?? string.Empty,
                Region = loc?.Region ?? string.Empty,
                Country = loc?.Country ?? string.Empty,
                LocalTime = loc?.LocalTime ?? string.Empty,

                Description = cur?.Condition?.Text ?? "No description",
                AirQuality = MapAirQuality(cur.AirQuality),

                Temperature = cur.TempC,
                TemperatureF = cur.TempF,
                FeelsLike = cur.FeelsLikeC,
                FeelsLikeF = cur.FeelsLikeF,
                Humidity = cur.Humidity,

                PressureMb = cur.PressureMb,
                PressureIn = cur.PressureIn,
                Cloud = cur.Cloud,
                Uv = cur.Uv,

                WindKph = cur.WindKph,
                WindMph = cur.WindMph,
                WindDegree = cur.WindDegree,
                WindDirection = cur.WindDir,
                GustKph = cur.GustKph,
                GustMph = cur.GustMph,

                VisibilityKm = cur.VisKm,
                VisibilityMiles = cur.VisMiles,

                IsDay = cur.IsDay == 1
            };
        }

        /// <summary>
        /// Calls the WeatherAPI forecast endpoint and creates a strongly typed <see cref="ForecastResult"/>.
        /// </summary>
        private async Task<ForecastResult?> GetForecastAsync(string url, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<WeatherApiForecastResponse>(cancellationToken: cancellationToken);

            if (apiResponse == null ||
                apiResponse.Forecast?.ForecastDay == null ||
                apiResponse.Forecast.ForecastDay.Count == 0)
            {
                return null;
            }

            var loc = apiResponse.Location;

            var result = new ForecastResult
            {
                City = loc?.Name ?? string.Empty,
                Region = loc?.Region ?? string.Empty,
                Country = loc?.Country ?? string.Empty
            };

            foreach (var day in apiResponse.Forecast.ForecastDay)
            {
                var d = day.Day;

                result.Days.Add(new DailyForecast
                {
                    Date = day.Date,

                    MaxTemp = d.MaxTempC,
                    MinTemp = d.MinTempC,
                    AvgTemp = d.AvgTempC,
                    MaxTempF = d.MaxTempF,
                    MinTempF = d.MinTempF,
                    AvgTempF = d.AvgTempF,

                    TotalPrecipMm = d.TotalPrecipMm,
                    TotalPrecipIn = d.TotalPrecipIn,
                    ChanceOfRain = d.DailyChanceOfRain,
                    ChanceOfSnow = d.DailyChanceOfSnow,

                    MaxWindKph = d.MaxWindKph,
                    MaxWindMph = d.MaxWindMph,
                    Uv = d.Uv,

                    AvgVisibilityKm = d.AvgVisKm,
                    AvgVisibilityMiles = d.AvgVisMiles,
                    AvgHumidity = d.AvgHumidity,

                    Condition = d.Condition?.Text ?? "No description",
                    AirQuality = MapAirQuality(d.AirQuality)
                });
            }

            return result;
        }

        /// <summary>
        /// Converts WeatherAPI's raw air-quality structure into the internal <see cref="AirQualityResult"/>.
        /// </summary>
        private AirQualityResult? MapAirQuality(WeatherApiAirQuality? src)
        {
            if (src == null) return null;

            return new AirQualityResult
            {
                Co = src.Co,
                O3 = src.O3,
                No2 = src.No2,
                So2 = src.So2,
                Pm2_5 = src.Pm2_5,
                Pm10 = src.Pm10,
                UsEpaIndex = src.UsEpaIndex,
                GbDefraIndex = src.GbDefraIndex
            };
        }

        // =============================================================================================
        // NEW FEATURES
        // =============================================================================================

        /// <summary>
        /// Retrieves time-zone information for the specified city or geographic query.
        /// </summary>
        public async Task<TimeZoneResult?> GetTimeZoneAsync(string query, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be empty.", nameof(query));

            var url = $"timezone.json?key={_options.ApiKey}&q={Uri.EscapeDataString(query)}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var apiResponse = await response.Content.ReadFromJsonAsync<WeatherApiTimeZoneResponse>(cancellationToken: cancellationToken);
            if (apiResponse?.Location == null)
                return null;

            var loc = apiResponse.Location;

            return new TimeZoneResult
            {
                Name = loc.TimeZoneId,
                Region = loc.Region,
                Country = loc.Country,
                Lat = loc.Lat,
                Lon = loc.Lon,
                LocalTime = loc.LocalTime,
                GmtOffsetHours = 0 // optional future enhancement
            };
        }

        /// <summary>
        /// Retrieves astronomy details (sunrise, sunset, moon, illumination) for a given location and date.
        /// </summary>
        public async Task<AstronomyResult?> GetAstronomyAsync(string query, DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be empty.", nameof(query));

            var dateStr = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var url = $"astronomy.json?key={_options.ApiKey}&q={Uri.EscapeDataString(query)}&dt={dateStr}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var apiResponse = await response.Content.ReadFromJsonAsync<WeatherApiAstronomyResponse>(
                cancellationToken: cancellationToken);

            if (apiResponse?.Location == null || apiResponse.Astronomy?.Astro == null)
                return null;

            var loc = apiResponse.Location;
            var astro = apiResponse.Astronomy.Astro;

            return new AstronomyResult
            {
                City = loc.Name,
                Region = loc.Region,
                Country = loc.Country,
                Date = date,

                Sunrise = astro.Sunrise,
                Sunset = astro.Sunset,
                Moonrise = astro.Moonrise,
                Moonset = astro.Moonset,
                MoonPhase = astro.MoonPhase,
                MoonIllumination = astro.MoonIllumination,
                IsMoonUp = astro.IsMoonUp == 1,
                IsSunUp = astro.IsSunUp == 1
            };
        }

        /// <summary>
        /// Retrieves location details associated with an IP address, including geographic location,
        /// ISP name, and inferred time zone.
        /// </summary>
        public async Task<IpLookupResult?> GetIpLookupAsync(string ip, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("IP cannot be empty.", nameof(ip));

            var url = $"ip.json?key={_options.ApiKey}&q={Uri.EscapeDataString(ip)}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            var apiResponse = await response.Content.ReadFromJsonAsync<WeatherApiIpLookupResponse>(
                cancellationToken: cancellationToken);

            if (apiResponse == null)
                return null;

            return new IpLookupResult
            {
                Ip = apiResponse.Ip,
                City = apiResponse.City,
                Region = apiResponse.Region,
                Country = apiResponse.Country,
                Lat = apiResponse.Lat,
                Lon = apiResponse.Lon,
                TimeZoneId = apiResponse.Tz_id,
                LocalTime = apiResponse.Localtime,
                Isp = apiResponse.Isp
            };
        }

        /// <summary>
        /// Performs a free-text location lookup and returns a list of matching locations.
        /// </summary>
        public async Task<List<SearchLocationResult>> SearchLocationsAsync(string query, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be empty.", nameof(query));

            var url = $"search.json?key={_options.ApiKey}&q={Uri.EscapeDataString(query)}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return new List<SearchLocationResult>();

            var apiResponse = await response.Content.ReadFromJsonAsync<List<WeatherApiSearchLocation>>(
                cancellationToken: cancellationToken);

            if (apiResponse == null || apiResponse.Count == 0)
                return new List<SearchLocationResult>();

            return apiResponse.Select(x => new SearchLocationResult
            {
                Id = x.Id,
                Name = x.Name,
                Region = x.Region,
                Country = x.Country,
                Lat = x.Lat,
                Lon = x.Lon,
                Url = x.Url
            }).ToList();
        }
    }
}
