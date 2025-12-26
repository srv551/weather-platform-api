using WeatherApi.Application.DTOs;
using WeatherApi.Application.Interfaces;

namespace WeatherApi.Infrastructure.Services
{
    /// <summary>
    /// Default implementation of occupation-based weather insights.
    /// </summary>
    public class OccupationWeatherService : IOccupationWeatherService
    {
        private readonly ITodaySummaryService _todaySummaryService;

        public OccupationWeatherService(ITodaySummaryService todaySummaryService)
        {
            _todaySummaryService = todaySummaryService;
        }

        public async Task<OccupationWeatherResult?> GetOccupationWeatherAsync(
            string city,
            string occupation,
            CancellationToken cancellationToken = default)
        {
            var summary = await _todaySummaryService.GetTodaySummaryAsync(city, cancellationToken);
            if (summary == null) return null;

            occupation = occupation.Trim().ToLowerInvariant();

            return occupation switch
            {
                "farmer" => BuildFarmerAdvice(summary),
                "officeworker" => BuildOfficeWorkerAdvice(summary),
                "delivery" => BuildDeliveryAdvice(summary),
                "construction" => BuildConstructionAdvice(summary),
                "traveler" => BuildTravelerAdvice(summary),
                _ => BuildGenericAdvice(summary, occupation)
            };
        }

        // ---------- Farmer ----------
        private static OccupationWeatherResult BuildFarmerAdvice(TodaySummaryResult s)
        {
            var opp = new List<string>();
            var risks = new List<string>();
            var actions = new List<string>();

            int score = 85;

            if (s.ForecastToday.TotalPrecipMm >= 5)
            {
                opp.Add("Rainfall may benefit soil moisture and crop hydration.");
                actions.Add("Delay irrigation to conserve water resources.");
            }

            if (s.ForecastToday.ChanceOfRain > 70)
            {
                risks.Add("High rain probability may damage flowering crops.");
                actions.Add("Ensure field drainage and protect sensitive crops.");
                score -= 15;
            }

            if (s.Current.FeelsLike > 38)
            {
                risks.Add("Heat stress risk for farm workers and crops.");
                actions.Add("Schedule farm work before 10 AM or after 4 PM.");
                score -= 20;
            }

            if (s.ForecastToday.MaxWindKph > 18)
            {
                risks.Add("High wind speeds may reduce spray effectiveness.");
                actions.Add("Avoid pesticide spraying today.");
                score -= 10;
            }

            return BuildResult(s, "Farmer", score, opp, risks, actions, "06:00 AM – 10:00 AM");
        }

        // ---------- Delivery ----------
        private static OccupationWeatherResult BuildDeliveryAdvice(TodaySummaryResult s)
        {
            var opp = new List<string>();
            var risks = new List<string>();
            var actions = new List<string>();

            int score = 80;

            if (s.Current.VisibilityKm < 4)
            {
                risks.Add("Low visibility increases accident risk.");
                actions.Add("Reduce speed and allow extra delivery time.");
                score -= 20;
            }

            if (s.ForecastToday.ChanceOfRain > 50)
            {
                risks.Add("Wet roads may slow deliveries.");
                actions.Add("Use waterproof packaging.");
                score -= 15;
            }

            if (s.Current.Uv < 5 && s.Current.FeelsLike < 32)
            {
                opp.Add("Comfortable weather supports efficient deliveries.");
            }

            return BuildResult(s, "DeliveryExecutive", score, opp, risks, actions, "09:00 AM – 12:00 PM");
        }

        // ---------- Office Worker ----------
        private static OccupationWeatherResult BuildOfficeWorkerAdvice(TodaySummaryResult s)
        {
            var opportunities = new List<string>
    {
        "Weather conditions are generally suitable for daily office commute."
    };

            var risks = new List<string>();
            var actions = new List<string>();

            int score = 85;
            string bestTime = "Morning commute hours";

            if (s.ForecastToday.ChanceOfRain > 40)
            {
                risks.Add("Rain during peak commute hours.");
                actions.Add("Carry an umbrella or rain jacket.");
                score -= 5;
            }

            if (s.Current.AirQuality?.UsEpaIndex >= 3)
            {
                risks.Add("Air quality may cause discomfort during commute.");
                actions.Add("Avoid long outdoor exposure during peak traffic hours.");
                score -= 5;
            }

            return BuildResult(
                s,
                "OfficeWorker",
                score,
                opportunities,
                risks,
                actions,
                bestTime
            );
        }

        // ---------- Construction ----------
        private static OccupationWeatherResult BuildConstructionAdvice(TodaySummaryResult s)
        {
            var opportunities = new List<string>
    {
        "Conditions allow for planned construction activities with precautions."
    };

            var risks = new List<string>();
            var actions = new List<string>();

            int score = 70;
            string bestTime = "Early morning (6–10 AM)";

            if (s.Current.FeelsLike > 36)
            {
                risks.Add("High heat stress risk for outdoor labor.");
                actions.Add("Schedule heavy work early morning and ensure hydration breaks.");
                score -= 20;
            }

            if (s.ForecastToday.ChanceOfRain > 50)
            {
                risks.Add("Rain may disrupt outdoor construction work.");
                actions.Add("Prepare protective covers and adjust schedules.");
                score -= 15;
            }

            if (s.Current.Uv >= 8)
            {
                risks.Add("Very high UV exposure.");
                actions.Add("Use protective clothing, helmets, and sunscreen.");
                score -= 10;
            }

            return BuildResult(
                s,
                "Construction",
                score,
                opportunities,
                risks,
                actions,
                bestTime
            );
        }

        // ---------- Traveler ----------
        private static OccupationWeatherResult BuildTravelerAdvice(TodaySummaryResult s)
        {
            var opportunities = new List<string>
    {
        "Weather is suitable for sightseeing and travel activities."
    };

            var risks = new List<string>();
            var actions = new List<string>();

            int score = 90;
            string bestTime = "Late morning to early evening";

            if (s.Current.Uv > 7)
            {
                risks.Add("High UV exposure during outdoor sightseeing.");
                actions.Add("Use sunscreen, hats, and sunglasses.");
                score -= 5;
            }

            if (s.ForecastToday.ChanceOfRain > 40)
            {
                risks.Add("Possible rain interruptions during travel.");
                actions.Add("Plan indoor attractions or keep rain gear ready.");
                score -= 10;
            }

            return BuildResult(
                s,
                "Traveler",
                score,
                opportunities,
                risks,
                actions,
                bestTime
            );
        }

        // ---------- Generic ----------
        private static OccupationWeatherResult BuildGenericAdvice(
            TodaySummaryResult s,
            string occupation)
        {
            var opportunities = new List<string>
    {
        "General weather conditions are acceptable for routine activities."
    };

            var risks = new List<string>();
            var actions = new List<string>();
            string bestTime = "Daytime hours";

            return BuildResult(
                s,
                occupation,
                75,
                opportunities,
                risks,
                actions,
                bestTime
            );
        }



        //private static OccupationWeatherResult BuildResult(
        //    TodaySummaryResult s,
        //    string occupation,
        //    int score,
        //    List<string> insights,
        //    List<string> warnings)
        //{
        //    score = Math.Clamp(score, 0, 100);

        //    return new OccupationWeatherResult
        //    {
        //        City = s.Current.City,
        //        Occupation = occupation,
        //        SuitabilityScore = score,
        //        SuitabilityLabel = score switch
        //        {
        //            >= 80 => "Excellent",
        //            >= 60 => "Good",
        //            >= 40 => "Moderate",
        //            _ => "Poor"
        //        },
        //        Insights = insights,
        //        Warnings = warnings
        //    };
        //}

        private static OccupationWeatherResult BuildResult(
        TodaySummaryResult summary,
        string occupation,
        int score,
        List<string> opportunities,
        List<string> risks,
        List<string> actions,
        string bestTime)
        {
            return new OccupationWeatherResult
            {
                City = summary.Current.City,
                Occupation = occupation,
                SuitabilityScore = Math.Clamp(score, 0, 100),
                SuitabilityLabel = score switch
                {
                    >= 80 => "Excellent",
                    >= 60 => "Good",
                    >= 40 => "Moderate",
                    _ => "Poor"
                },
                Opportunities = opportunities,
                Risks = risks,
                RecommendedActions = actions,
                BestTimeWindow = bestTime
            };
        }

    }
}
