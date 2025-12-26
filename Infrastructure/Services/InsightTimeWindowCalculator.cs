using WeatherApi.Application.DTOs;

namespace WeatherApi.Infrastructure.Services
{
    internal static class InsightTimeWindowCalculator
    {
        public static (string Best, string Worst) Calculate(
            WeatherResult current,
            DailyForecast forecast,
            AstronomyResult astronomy)
        {
            // Default windows
            var best = "Morning (6:00–9:00 AM)";
            var worst = "Afternoon (12:00–4:00 PM)";

            // Adjust best time based on sunrise
            if (TryParseHour(astronomy.Sunrise, out var sunriseHour))
            {
                var start = Math.Max(sunriseHour + 1, 6);
                best = $"Morning ({start}:00–{start + 3}:00)";
            }

            // Heat-driven worst window
            if (current.FeelsLike >= 35 || forecast.MaxTemp >= 35)
            {
                worst = "Midday to afternoon (12:00–5:00 PM)";
            }

            // Rain-driven worst window
            if (forecast.ChanceOfRain >= 60)
            {
                worst = "Late afternoon to evening (3:00–7:00 PM)";
            }

            // Extreme UV overrides everything
            if (current.Uv >= 9)
            {
                worst = "Late morning to afternoon (11:00 AM–4:00 PM)";
            }

            return (best, worst);
        }

        private static bool TryParseHour(string time, out int hour)
        {
            hour = 0;

            if (string.IsNullOrWhiteSpace(time))
                return false;

            // Handles formats like "06:45 AM"
            if (DateTime.TryParse(time, out var parsed))
            {
                hour = parsed.Hour;
                return true;
            }

            return false;
        }
    }
}
