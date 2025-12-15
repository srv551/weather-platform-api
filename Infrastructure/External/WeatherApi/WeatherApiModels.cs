using System.Text.Json.Serialization;

namespace WeatherApi.Infrastructure.External.WeatherApi
{
    /// <summary>
    /// Represents the raw location object returned by WeatherAPI.
    /// Used across current, forecast, time zone, astronomy, and IP lookup responses.
    /// </summary>
    public class WeatherApiLocation
    {
        /// <summary>
        /// Name of the location (city/town).
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Administrative region (state, province, region).
        /// </summary>
        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Country name.
        /// </summary>
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Latitude in decimal degrees.
        /// </summary>
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        /// <summary>
        /// Longitude in decimal degrees.
        /// </summary>
        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        /// <summary>
        /// Time zone identifier (e.g., "Asia/Kolkata").
        /// </summary>
        [JsonPropertyName("tz_id")]
        public string TimeZoneId { get; set; } = string.Empty;

        /// <summary>
        /// Local time at the location.
        /// </summary>
        [JsonPropertyName("localtime")]
        public string LocalTime { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the "condition" block in WeatherAPI responses (text, icon, code).
    /// </summary>
    public class WeatherApiCondition
    {
        /// <summary>
        /// Short textual description of the conditions (e.g., "Partly cloudy").
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Location of the condition icon.
        /// </summary>
        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Internal WeatherAPI condition code.
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }
    }

    // -------------------------------------------------------------------------
    // CURRENT WEATHER (current.json)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Root object for the WeatherAPI "current.json" response.
    /// </summary>
    public class WeatherApiCurrentResponse
    {
        [JsonPropertyName("location")]
        public WeatherApiLocation Location { get; set; } = new();

        [JsonPropertyName("current")]
        public WeatherApiCurrent Current { get; set; } = new();
    }

    /// <summary>
    /// Represents the "current" block in the WeatherAPI response.
    /// Contains temperature, wind, humidity, visibility, and more.
    /// </summary>
    public class WeatherApiCurrent
    {
        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("temp_f")]
        public double TempF { get; set; }

        [JsonPropertyName("feelslike_c")]
        public double FeelsLikeC { get; set; }

        [JsonPropertyName("feelslike_f")]
        public double FeelsLikeF { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("pressure_mb")]
        public double PressureMb { get; set; }

        [JsonPropertyName("pressure_in")]
        public double PressureIn { get; set; }

        [JsonPropertyName("cloud")]
        public int Cloud { get; set; }

        [JsonPropertyName("uv")]
        public double Uv { get; set; }

        [JsonPropertyName("wind_kph")]
        public double WindKph { get; set; }

        [JsonPropertyName("wind_mph")]
        public double WindMph { get; set; }

        [JsonPropertyName("wind_degree")]
        public int WindDegree { get; set; }

        [JsonPropertyName("wind_dir")]
        public string WindDir { get; set; } = string.Empty;

        [JsonPropertyName("gust_kph")]
        public double GustKph { get; set; }

        [JsonPropertyName("gust_mph")]
        public double GustMph { get; set; }

        [JsonPropertyName("vis_km")]
        public double VisKm { get; set; }

        [JsonPropertyName("vis_miles")]
        public double VisMiles { get; set; }

        /// <summary>
        /// Indicates whether it is daytime (1 = day, 0 = night).
        /// </summary>
        [JsonPropertyName("is_day")]
        public int IsDay { get; set; }

        [JsonPropertyName("condition")]
        public WeatherApiCondition Condition { get; set; } = new();

        /// <summary>
        /// Optional air-quality block (only returned when the provider supports it).
        /// </summary>
        [JsonPropertyName("air_quality")]
        public WeatherApiAirQuality? AirQuality { get; set; }
    }

    // -------------------------------------------------------------------------
    // FORECAST (forecast.json)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Root object for the WeatherAPI "forecast.json" response.
    /// </summary>
    public class WeatherApiForecastResponse
    {
        [JsonPropertyName("location")]
        public WeatherApiLocation Location { get; set; } = new();

        [JsonPropertyName("forecast")]
        public WeatherApiForecast Forecast { get; set; } = new();
    }

    /// <summary>
    /// Represents the outer "forecast" element containing forecast days.
    /// </summary>
    public class WeatherApiForecast
    {
        [JsonPropertyName("forecastday")]
        public List<WeatherApiForecastDay> ForecastDay { get; set; } = new();
    }

    /// <summary>
    /// Represents a single forecast day (date + weather summary).
    /// </summary>
    public class WeatherApiForecastDay
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("day")]
        public WeatherApiDay Day { get; set; } = new();
    }

    /// <summary>
    /// Weather summary for a single forecast day.
    /// Includes temperature, precipitation, visibility, humidity, UV, and condition.
    /// </summary>
    public class WeatherApiDay
    {
        [JsonPropertyName("maxtemp_c")]
        public double MaxTempC { get; set; }

        [JsonPropertyName("maxtemp_f")]
        public double MaxTempF { get; set; }

        [JsonPropertyName("mintemp_c")]
        public double MinTempC { get; set; }

        [JsonPropertyName("mintemp_f")]
        public double MinTempF { get; set; }

        [JsonPropertyName("avgtemp_c")]
        public double AvgTempC { get; set; }

        [JsonPropertyName("avgtemp_f")]
        public double AvgTempF { get; set; }

        [JsonPropertyName("maxwind_kph")]
        public double MaxWindKph { get; set; }

        [JsonPropertyName("maxwind_mph")]
        public double MaxWindMph { get; set; }

        [JsonPropertyName("totalprecip_mm")]
        public double TotalPrecipMm { get; set; }

        [JsonPropertyName("totalprecip_in")]
        public double TotalPrecipIn { get; set; }

        [JsonPropertyName("daily_chance_of_rain")]
        public int DailyChanceOfRain { get; set; }

        [JsonPropertyName("daily_chance_of_snow")]
        public int DailyChanceOfSnow { get; set; }

        [JsonPropertyName("avgvis_km")]
        public double AvgVisKm { get; set; }

        [JsonPropertyName("avgvis_miles")]
        public double AvgVisMiles { get; set; }

        [JsonPropertyName("avghumidity")]
        public double AvgHumidity { get; set; }

        [JsonPropertyName("uv")]
        public double Uv { get; set; }

        [JsonPropertyName("condition")]
        public WeatherApiCondition Condition { get; set; } = new();

        /// <summary>
        /// Optional air-quality data for the forecast day.
        /// </summary>
        [JsonPropertyName("air_quality")]
        public WeatherApiAirQuality? AirQuality { get; set; }
    }

    // -------------------------------------------------------------------------
    // TIME ZONE (timezone.json)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Represents the WeatherAPI "timezone.json" response.
    /// Reuses the standard location model.
    /// </summary>
    public class WeatherApiTimeZoneResponse
    {
        [JsonPropertyName("location")]
        public WeatherApiLocation Location { get; set; } = new();
    }

    // -------------------------------------------------------------------------
    // ASTRONOMY (astronomy.json)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Represents the WeatherAPI "astronomy.json" response.
    /// </summary>
    public class WeatherApiAstronomyResponse
    {
        [JsonPropertyName("location")]
        public WeatherApiLocation Location { get; set; } = new();

        [JsonPropertyName("astronomy")]
        public WeatherApiAstronomy Astronomy { get; set; } = new();
    }

    /// <summary>
    /// Wrapper object for the "astro" element.
    /// </summary>
    public class WeatherApiAstronomy
    {
        [JsonPropertyName("astro")]
        public WeatherApiAstro Astro { get; set; } = new();
    }

    /// <summary>
    /// Represents astronomy details for sunrise, sunset, moon phase, and illumination.
    /// </summary>
    public class WeatherApiAstro
    {
        [JsonPropertyName("sunrise")]
        public string Sunrise { get; set; } = string.Empty;

        [JsonPropertyName("sunset")]
        public string Sunset { get; set; } = string.Empty;

        [JsonPropertyName("moonrise")]
        public string Moonrise { get; set; } = string.Empty;

        [JsonPropertyName("moonset")]
        public string Moonset { get; set; } = string.Empty;

        [JsonPropertyName("moon_phase")]
        public string MoonPhase { get; set; } = string.Empty;

        [JsonPropertyName("moon_illumination")]
        public int MoonIllumination { get; set; }

        [JsonPropertyName("is_moon_up")]
        public int IsMoonUp { get; set; }

        [JsonPropertyName("is_sun_up")]
        public int IsSunUp { get; set; }
    }

    // -------------------------------------------------------------------------
    // IP LOOKUP (ip.json)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Represents the response from WeatherAPI's IP lookup endpoint.
    /// </summary>
    public class WeatherApiIpLookupResponse
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("tz_id")]
        public string Tz_id { get; set; } = string.Empty;

        [JsonPropertyName("localtime")]
        public string Localtime { get; set; } = string.Empty;

        [JsonPropertyName("isp")]
        public string Isp { get; set; } = string.Empty;
    }

    // -------------------------------------------------------------------------
    // SEARCH / AUTOCOMPLETE (search.json)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Raw search/autocomplete location returned by WeatherAPI.
    /// </summary>
    public class WeatherApiSearchLocation
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    // -------------------------------------------------------------------------
    // AIR QUALITY (raw WeatherAPI model)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Represents raw air-quality pollutant data returned by WeatherAPI.
    /// Includes pollutant concentrations and EPA/DEFRA index values.
    /// </summary>
    public class WeatherApiAirQuality
    {
        /// <summary>
        /// Carbon monoxide concentration (μg/m3).
        /// </summary>
        [JsonPropertyName("co")]
        public double Co { get; set; }

        /// <summary>
        /// Ozone concentration (μg/m3).
        /// </summary>
        [JsonPropertyName("o3")]
        public double O3 { get; set; }

        /// <summary>
        /// Nitrogen dioxide concentration (μg/m3).
        /// </summary>
        [JsonPropertyName("no2")]
        public double No2 { get; set; }

        /// <summary>
        /// Sulfur dioxide concentration (μg/m3).
        /// </summary>
        [JsonPropertyName("so2")]
        public double So2 { get; set; }

        /// <summary>
        /// Particulate matter PM2.5 concentration (μg/m3).
        /// </summary>
        [JsonPropertyName("pm2_5")]
        public double Pm2_5 { get; set; }

        /// <summary>
        /// Particulate matter PM10 concentration (μg/m3).
        /// </summary>
        [JsonPropertyName("pm10")]
        public double Pm10 { get; set; }

        /// <summary>
        /// US EPA AQI index (1–6 scale).
        /// </summary>
        [JsonPropertyName("us-epa-index")]
        public int UsEpaIndex { get; set; }

        /// <summary>
        /// UK DEFRA AQI index.
        /// </summary>
        [JsonPropertyName("gb-defra-index")]
        public int GbDefraIndex { get; set; }
    }
}
