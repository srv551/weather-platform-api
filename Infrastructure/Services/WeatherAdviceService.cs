using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Infrastructure.Services
{
    /// <summary>
    /// Default implementation of <see cref="IWeatherAdviceService"/> that generates
    /// high-level weather advice for a location. Recommendations are derived from
    /// current conditions, today's forecast, and air-quality information where available.
    /// </summary>
    public class WeatherAdviceService : IWeatherAdviceService
    {
        private readonly ITodaySummaryService _todaySummaryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherAdviceService"/> class.
        /// </summary>
        /// <param name="todaySummaryService">
        /// Service responsible for providing combined weather, forecast, and astronomy data.
        /// </param>
        public WeatherAdviceService(ITodaySummaryService todaySummaryService)
        {
            _todaySummaryService = todaySummaryService;
        }

        /// <summary>
        /// Generates weather-based advice for the specified city, including umbrella guidance,
        /// heat precautions, UV protection recommendations, and air-quality advisories.
        /// </summary>
        /// <param name="city">City name or "City,CountryCode" identifier (e.g., "Delhi,IN").</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// A <see cref="WeatherAdviceResult"/> containing weather advisories, or <c>null</c>
        /// if required summary data is unavailable.
        /// </returns>
        public async Task<WeatherAdviceResult?> GetAdviceAsync(string city, CancellationToken cancellationToken = default)
        {
            var summary = await _todaySummaryService.GetTodaySummaryAsync(city, cancellationToken);
            if (summary == null)
                return null;

            var current = summary.Current;
            var today = summary.ForecastToday;

            var advice = new WeatherAdviceResult
            {
                ChanceOfRain = today.ChanceOfRain,
                TotalPrecipMm = today.TotalPrecipMm
            };

            // --------------------------------------------------------------------
            // UMBRELLA LOGIC
            // --------------------------------------------------------------------
            // Decides whether the user should carry an umbrella based on chance of rain
            // and expected precipitation amounts.
            if (today.ChanceOfRain >= 60 || today.TotalPrecipMm >= 2)
            {
                advice.ShouldCarryUmbrella = true;
                advice.UmbrellaReason =
                    $"Chance of rain is {today.ChanceOfRain}% with {today.TotalPrecipMm:0.0}mm precipitation expected.";
            }
            else if (today.ChanceOfRain >= 30)
            {
                advice.ShouldCarryUmbrella = true;
                advice.UmbrellaReason =
                    $"Moderate chance of rain today ({today.ChanceOfRain}%). A compact umbrella is recommended.";
            }
            else
            {
                advice.ShouldCarryUmbrella = false;
                advice.UmbrellaReason =
                    $"Low chance of rain today ({today.ChanceOfRain}%). Umbrella is optional.";
            }

            // --------------------------------------------------------------------
            // HEAT WARNING
            // --------------------------------------------------------------------
            if (current.Temperature >= 35 || current.FeelsLike >= 37)
            {
                advice.HeatWarning = true;
            }

            // --------------------------------------------------------------------
            // UV WARNING
            // --------------------------------------------------------------------
            if (current.Uv >= 8)
            {
                advice.UvWarning = true;
            }

            // --------------------------------------------------------------------
            // AIR QUALITY ANALYSIS
            // --------------------------------------------------------------------
            // Prefer current AQI when available; fall back to forecast AQI.
            var aq = current.AirQuality ?? today.AirQuality;
            if (aq != null)
            {
                advice.AirQualityIndex = aq.UsEpaIndex;

                // Interpret US EPA AQI scale (1–6)
                if (aq.UsEpaIndex >= 4)
                {
                    advice.AirQualityWarning = true;
                    advice.AirQualityNotes =
                        $"Air quality is poor (US EPA index {aq.UsEpaIndex}). Limit outdoor exertion and consider wearing a mask.";
                }
                else if (aq.UsEpaIndex == 3)
                {
                    advice.AirQualityWarning = false;
                    advice.AirQualityNotes =
                        $"Air quality is moderate/unhealthy for sensitive groups (US EPA index {aq.UsEpaIndex}). Sensitive people should take precautions.";
                }
                else
                {
                    advice.AirQualityWarning = false;
                    advice.AirQualityNotes = $"Air quality is acceptable (US EPA index {aq.UsEpaIndex}).";
                }

                // Additional PM2.5 check for detailed particulate warnings
                try
                {
                    var pm25 = aq.Pm2_5;
                    if (pm25 > 35)
                    {
                        advice.AirQualityWarning = true;
                        var pmNote = $"PM2.5 is {pm25:0.0} µg/m³ which is elevated.";
                        advice.AirQualityNotes = string.IsNullOrWhiteSpace(advice.AirQualityNotes)
                            ? pmNote
                            : $"{advice.AirQualityNotes} {pmNote}";
                    }
                }
                catch
                {
                    // If pollutant fields missing, silently ignore
                }
            }

            // --------------------------------------------------------------------
            // FINAL NOTES COMPILATION
            // --------------------------------------------------------------------
            var notes = new List<string>();

            if (advice.HeatWarning)
                notes.Add("Stay hydrated and avoid outdoor activity during peak afternoon hours.");

            if (advice.UvWarning)
                notes.Add("Use sunscreen and wear sunglasses if going out in the afternoon.");

            if (advice.AirQualityWarning)
                notes.Add(advice.AirQualityNotes);
            else if (!string.IsNullOrWhiteSpace(advice.AirQualityNotes))
                notes.Add(advice.AirQualityNotes);

            if (!advice.HeatWarning && !advice.UvWarning && advice.ShouldCarryUmbrella)
            {
                notes.Add("Weather is otherwise comfortable; just be prepared for a possible shower.");
            }

            advice.Notes = string.Join(" ", notes).Trim();

            return advice;
        }
    }
}
