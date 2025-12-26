using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Infrastructure.Services
{
    /// <summary>
    /// Provides functionality to calculate a simple travel or comfort score for a city.
    /// The score is derived from temperature, rain probability, UV index, and air quality
    /// using today's combined weather summary.
    /// </summary>
    public class TravelScoreService : ITravelScoreService
    {
        private readonly ITodaySummaryService _todaySummaryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TravelScoreService"/> class.
        /// </summary>
        /// <param name="todaySummaryService">
        /// Service used to fetch current weather, forecast, and astronomy information.
        /// </param>
        public TravelScoreService(ITodaySummaryService todaySummaryService)
        {
            _todaySummaryService = todaySummaryService;
        }

        /// <summary>
        /// Calculates today's travel or comfort score for a specified city.
        /// The score is composed of temperature, rain, UV exposure, and air-quality components.
        /// </summary>
        /// <param name="city">City for which the score should be calculated.</param>
        /// <param name="cancellationToken">Token to observe cancellation.</param>
        /// <returns>
        /// A populated <see cref="TravelScoreResult"/> instance if the summary could be retrieved,
        /// or <c>null</c> when insufficient data is available.
        /// </returns>
        public async Task<TravelScoreResult?> GetTravelScoreAsync(
            string city,
            CancellationToken cancellationToken = default)
        {
            var summary = await _todaySummaryService.GetTodaySummaryAsync(city, cancellationToken);
            if (summary == null)
                return null;

            var current = summary.Current;
            var today = summary.ForecastToday;
            var aq = current.AirQuality ?? today.AirQuality;

            var warnings = new List<string>();

            // Compute category scores
            int tempScore = ScoreTemperature(current.FeelsLike, warnings);
            int rainScore = ScoreRain(today.ChanceOfRain, today.TotalPrecipMm, warnings);
            int uvScore = ScoreUv(current.Uv, warnings);
            int aqiScore = ScoreAirQuality(aq, warnings);

            // Combine scores (0–100)
            int overall = Math.Clamp(tempScore + rainScore + uvScore + aqiScore, 0, 100);

            // Label mapping
            string label = overall switch
            {
                >= 80 => "Excellent",
                >= 60 => "Good",
                >= 40 => "OK",
                _ => "Poor"
            };

            string summaryText = BuildSummaryText(overall, label, warnings);

            return new TravelScoreResult
            {
                City = current.City,
                Region = current.Region,
                Country = current.Country,
                OverallScore = overall,
                ComfortLabel = label,
                Summary = summaryText,
                TemperatureScore = tempScore,
                RainScore = rainScore,
                UvScore = uvScore,
                AirQualityScore = aqiScore,
                Warnings = warnings
            };
        }

        public async Task<TravelScoreExplanationResult?> ExplainTravelScoreAsync(
    string city,
    CancellationToken cancellationToken = default)
        {
            var score = await GetTravelScoreAsync(city, cancellationToken);
            if (score == null)
                return null;

            var breakdown = new Dictionary<string, string>
            {
                ["Temperature"] = ExplainTemperature(score.TemperatureScore),
                ["Rain"] = ExplainRain(score.RainScore),
                ["UV Index"] = ExplainUv(score.UvScore),
                ["Air Quality"] = ExplainAirQuality(score.AirQualityScore)
            };

            return new TravelScoreExplanationResult
            {
                City = score.City,
                OverallScore = score.OverallScore,
                ComfortLabel = score.ComfortLabel,
                Confidence = "High",
                Breakdown = breakdown,
                Explanation =
                    $"Today's travel score is {score.OverallScore}/100 ({score.ComfortLabel}). " +
                    $"Weather conditions are assessed based on temperature, rain probability, UV exposure, and air quality."
            };
        }

        private static string ExplainTemperature(int score) =>
            score switch
            {
                >= 22 => "Comfortable temperature range for most activities.",
                >= 15 => "Slightly warm or cool but generally manageable.",
                _ => "Temperature may feel uncomfortable for prolonged outdoor activity."
            };

        private static string ExplainRain(int score) =>
            score switch
            {
                >= 22 => "Low chance of rain with minimal disruption.",
                >= 15 => "Some chance of showers; minor inconvenience possible.",
                _ => "High likelihood of rain affecting outdoor plans."
            };

        private static string ExplainUv(int score) =>
            score switch
            {
                >= 22 => "Low to moderate UV levels.",
                >= 15 => "Elevated UV; sun protection recommended.",
                _ => "High UV exposure risk; avoid prolonged sun exposure."
            };

        private static string ExplainAirQuality(int score) =>
            score switch
            {
                >= 22 => "Air quality is good and suitable for outdoor activities.",
                >= 15 => "Moderate air quality; sensitive individuals should be cautious.",
                _ => "Poor air quality; outdoor exertion is not recommended."
            };


        /// <summary>
        /// Computes the temperature component of the travel score (0–25).
        /// </summary>
        /// <param name="feelsLikeC">The perceived temperature in Celsius.</param>
        /// <param name="warnings">A collection of warnings to append to if needed.</param>
        private static int ScoreTemperature(double feelsLikeC, List<string> warnings)
        {
            // Ideal comfort window: 20–30°C
            if (feelsLikeC >= 22 && feelsLikeC <= 28)
                return 25;

            if (feelsLikeC >= 18 && feelsLikeC < 22)
                return 22;

            if (feelsLikeC > 28 && feelsLikeC <= 32)
            {
                warnings.Add("It may feel a bit warm; plan lighter clothing.");
                return 20;
            }

            if (feelsLikeC >= 15 && feelsLikeC < 18)
                return 18;

            if (feelsLikeC > 32)
            {
                warnings.Add("High temperatures expected; stay hydrated and avoid peak afternoon outdoor activity.");
                return 10;
            }

            if (feelsLikeC < 10)
            {
                warnings.Add("Weather is quite cool; carry warm clothing.");
                return 8;
            }

            // 10–15°C
            warnings.Add("Slightly cool conditions; a light jacket may be needed.");
            return 15;
        }

        /// <summary>
        /// Computes the rain contribution to the travel score (0–25).
        /// </summary>
        /// <param name="chanceOfRain">Chance of rain in percent.</param>
        /// <param name="totalPrecipMm">Expected precipitation in millimeters.</param>
        /// <param name="warnings">List of warnings to append rainfall concerns.</param>
        private static int ScoreRain(int chanceOfRain, double totalPrecipMm, List<string> warnings)
        {
            if (chanceOfRain <= 10 && totalPrecipMm < 0.5)
                return 25;

            if (chanceOfRain <= 30 && totalPrecipMm < 2)
                return 22;

            if (chanceOfRain <= 50 && totalPrecipMm < 4)
            {
                warnings.Add("There is a chance of light showers; consider carrying a compact umbrella.");
                return 18;
            }

            if (chanceOfRain <= 70)
            {
                warnings.Add("Rain is likely; plan indoor alternatives or carry rain protection.");
                return 12;
            }

            warnings.Add("High risk of rain or heavy showers; outdoor plans may be disrupted.");
            return 6;
        }

        /// <summary>
        /// Computes the UV exposure component of the score (0–25).
        /// </summary>
        /// <param name="uv">UV index value.</param>
        /// <param name="warnings">List to append UV warnings to.</param>
        private static int ScoreUv(double uv, List<string> warnings)
        {
            if (uv <= 2)
                return 25;

            if (uv <= 5)
                return 22;

            if (uv <= 7)
            {
                warnings.Add("High UV index; use sunscreen and wear a hat/sunglasses.");
                return 16;
            }

            if (uv <= 10)
            {
                warnings.Add("Very high UV index; avoid midday sun and use strong sun protection.");
                return 10;
            }

            warnings.Add("Extreme UV index; minimise direct sun exposure.");
            return 6;
        }

        /// <summary>
        /// Computes the air quality component of the travel score (0–25),
        /// using US EPA index and PM2.5 thresholds where available.
        /// </summary>
        /// <param name="aq">Air quality measurements, or null if not provided.</param>
        /// <param name="warnings">List to append AQI warnings.</param>
        private static int ScoreAirQuality(AirQualityResult? aq, List<string> warnings)
        {
            if (aq == null)
                return 20; // Neutral score when data missing

            int score = 20;

            // Use US EPA index primarily
            if (aq.UsEpaIndex is int epa && epa >= 1)
            {
                score = epa switch
                {
                    1 => 25,
                    2 => 22,
                    3 => 18,
                    4 => 10,
                    5 => 6,
                    6 => 2,
                    _ => 20
                };

                if (epa >= 3)
                    warnings.Add($"Air quality is not ideal (US EPA index {epa}). Sensitive individuals should take extra care.");

                if (epa >= 4)
                    warnings.Add("Poor air quality; consider limiting outdoor exertion and using a mask.");
            }

            // PM2.5 fine particulate adjustment
            if (aq.Pm2_5 > 35)
            {
                warnings.Add($"PM2.5 is elevated at {aq.Pm2_5:0.0} µg/m³.");
                score = Math.Min(score, 10);
            }

            if (aq.Pm2_5 > 55)
            {
                warnings.Add("Very high fine particulate levels; outdoor activity is not recommended.");
                score = Math.Min(score, 6);
            }

            return Math.Clamp(score, 0, 25);
        }

        /// <summary>
        /// Builds the final summary message for the travel score, incorporating
        /// qualitative labels and collected warnings.
        /// </summary>
        /// <param name="overall">Combined travel score.</param>
        /// <param name="label">Comfort label derived from the score.</param>
        /// <param name="warnings">List of advisories to include.</param>
        private static string BuildSummaryText(int overall, string label, List<string> warnings)
        {
            var baseSummary = label switch
            {
                "Excellent" => "Excellent day for travelling and outdoor plans.",
                "Good" => "Overall good conditions for travelling with only minor considerations.",
                "OK" => "Conditions are acceptable, but review the notes before planning full-day outdoor activities.",
                "Poor" => "Conditions are not ideal for travel-focused outdoor plans.",
                _ => "Conditions vary; review warnings before planning your day."
            };

            if (!warnings.Any())
                return baseSummary;

            return $"{baseSummary} Key points: {string.Join(" ", warnings)}";
        }
    }
}
