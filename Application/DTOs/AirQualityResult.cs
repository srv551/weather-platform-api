namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents simplified air-quality metrics mapped from WeatherAPI's `air_quality` block.
    /// All concentration values are expressed in μg/m³.
    /// </summary>
    public class AirQualityResult
    {
        /// <summary>Carbon monoxide concentration (μg/m³).</summary>
        public double Co { get; set; }

        /// <summary>Ozone concentration (μg/m³).</summary>
        public double O3 { get; set; }

        /// <summary>Nitrogen dioxide concentration (μg/m³).</summary>
        public double No2 { get; set; }

        /// <summary>Sulfur dioxide concentration (μg/m³).</summary>
        public double So2 { get; set; }

        /// <summary>Fine particulate matter PM2.5 concentration (μg/m³).</summary>
        public double Pm2_5 { get; set; }

        /// <summary>Particulate matter PM10 concentration (μg/m³).</summary>
        public double Pm10 { get; set; }

        /// <summary>
        /// US EPA Air Quality Index (1–6), where 1 = Good and 6 = Hazardous.
        /// </summary>
        public int UsEpaIndex { get; set; }

        /// <summary>
        /// UK DEFRA Index (1–10), standard DEFRA scale.
        /// </summary>
        public int GbDefraIndex { get; set; }
    }
}
