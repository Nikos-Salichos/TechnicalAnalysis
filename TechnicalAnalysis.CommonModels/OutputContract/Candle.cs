using System.Text.Json.Serialization;
using TechnicalAnalysis.CommonModels.Indicators.Advanced;

namespace TechnicalAnalysis.CommonModels.OutputContract
{
    public class Candle
    {
        [JsonPropertyName("OPEN")]
        public decimal? Open { get; init; }

        [JsonPropertyName("HIGH")]
        public decimal? High { get; init; }

        [JsonPropertyName("LOW")]
        public decimal? Low { get; init; }

        [JsonPropertyName("CLOSE")]
        public decimal? Close { get; init; }

        [JsonPropertyName("OPENED_AT")]
        public string OpenedAt { get; init; }

        public List<EnhancedScan> EnhancedScans { get; init; } = [];
    }
}
