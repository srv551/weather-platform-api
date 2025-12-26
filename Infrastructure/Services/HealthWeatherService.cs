using WeatherApi.Application.Domain;
using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Infrastructure.Services
{
    public class HealthWeatherService : IHealthWeatherService
    {
        private readonly ITodaySummaryService _todaySummaryService;

        public HealthWeatherService(ITodaySummaryService todaySummaryService)
        {
            _todaySummaryService = todaySummaryService;
        }

        public async Task<HealthWeatherResult?> GetHealthInsightAsync(
            string city,
            HealthConditionType condition,
            CancellationToken cancellationToken = default)
        {
            var summary = await _todaySummaryService.GetTodaySummaryAsync(city, cancellationToken);
            if (summary == null)
                return null;

            return condition switch
            {
                HealthConditionType.Asthma => BuildAsthmaInsight(summary),
                HealthConditionType.HeartCondition => BuildHeartInsight(summary),
                HealthConditionType.Migraine => BuildMigraineInsight(summary),
                HealthConditionType.HeatSensitivity => BuildHeatInsight(summary),
                HealthConditionType.ColdSensitivity => BuildColdInsight(summary),
                HealthConditionType.Elderly => BuildElderlyInsight(summary),
                _ => null
            };
        }

        // ---------------- ASTHMA ----------------
        private static HealthWeatherResult BuildAsthmaInsight(TodaySummaryResult s)
        {
            var triggers = new List<string>();
            var recs = new List<string>();
            int risk = 30;

            var aq = s.Current.AirQuality ?? s.ForecastToday.AirQuality;

            if (aq?.UsEpaIndex >= 3)
            {
                triggers.Add($"Poor air quality (US EPA index {aq.UsEpaIndex}).");
                recs.Add("Limit outdoor exposure and carry prescribed inhaler.");
                risk += 30;
            }

            if (aq?.Pm2_5 > 35)
            {
                triggers.Add($"High PM2.5 levels ({aq.Pm2_5:0.0} µg/m³).");
                recs.Add("Avoid outdoor exercise and use a mask if needed.");
                risk += 20;
            }

            return BuildHealthResult(s, "Asthma", risk, triggers, recs, "Early morning only");
        }

        // ---------------- HEART ----------------
        private static HealthWeatherResult BuildHeartInsight(TodaySummaryResult s)
        {
            var triggers = new List<string>();
            var recs = new List<string>();
            int risk = 35;

            if (s.Current.FeelsLike > 35)
            {
                triggers.Add("High heat stress.");
                recs.Add("Avoid strenuous activity and stay hydrated.");
                risk += 30;
            }

            if (s.Current.Uv > 7)
            {
                triggers.Add("High UV exposure.");
                recs.Add("Avoid midday sun exposure.");
                risk += 10;
            }

            return BuildHealthResult(s, "HeartCondition", risk, triggers, recs, "Morning or evening");
        }

        // ---------------- MIGRAINE ----------------
        private static HealthWeatherResult BuildMigraineInsight(TodaySummaryResult s)
        {
            var triggers = new List<string>();
            var recs = new List<string>();
            int risk = 25;

            if (s.Current.PressureMb < 1000)
            {
                triggers.Add("Low atmospheric pressure.");
                recs.Add("Minimize screen exposure and rest in dim environments.");
                risk += 20;
            }

            return BuildHealthResult(s, "Migraine", risk, triggers, recs, "Late morning");
        }

        // ---------------- HEAT ----------------
        private static HealthWeatherResult BuildHeatInsight(TodaySummaryResult s)
        {
            var triggers = new List<string>();
            var recs = new List<string>();
            int risk = 40;

            if (s.Current.FeelsLike > 38)
            {
                triggers.Add("Extreme heat conditions.");
                recs.Add("Stay indoors during peak hours and hydrate frequently.");
                risk += 40;
            }

            return BuildHealthResult(s, "HeatSensitivity", risk, triggers, recs, "Before 9 AM");
        }

        // ---------------- COLD ----------------
        private static HealthWeatherResult BuildColdInsight(TodaySummaryResult s)
        {
            var triggers = new List<string>();
            var recs = new List<string>();
            int risk = 20;

            if (s.Current.Temperature < 8)
            {
                triggers.Add("Cold temperature exposure.");
                recs.Add("Wear layered clothing and limit prolonged exposure.");
                risk += 30;
            }

            return BuildHealthResult(s, "ColdSensitivity", risk, triggers, recs, "Midday");
        }

        // ---------------- ELDERLY ----------------
        private static HealthWeatherResult BuildElderlyInsight(TodaySummaryResult s)
        {
            var triggers = new List<string>();
            var recs = new List<string>();
            int risk = 35;

            if (s.Current.FeelsLike > 35 || s.Current.Temperature < 10)
            {
                triggers.Add("Temperature extremes.");
                recs.Add("Ensure temperature-controlled environments.");
                risk += 25;
            }

            return BuildHealthResult(s, "Elderly", risk, triggers, recs, "Late morning");
        }

        // ---------------- BUILDER ----------------
        private static HealthWeatherResult BuildHealthResult(
            TodaySummaryResult s,
            string condition,
            int risk,
            List<string> triggers,
            List<string> recs,
            string safeTime)
        {
            risk = Math.Clamp(risk, 0, 100);

            return new HealthWeatherResult
            {
                City = s.Current.City,
                HealthCondition = condition,
                RiskScore = risk,
                RiskLevel = risk switch
                {
                    >= 75 => "High",
                    >= 50 => "Moderate",
                    _ => "Low"
                },
                Triggers = triggers,
                Recommendations = recs,
                SafeTimeWindow = safeTime
            };
        }
    }
}
