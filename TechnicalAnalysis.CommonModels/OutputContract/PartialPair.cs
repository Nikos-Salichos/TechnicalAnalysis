using System.Text.Json.Serialization;

namespace TechnicalAnalysis.CommonModels.OutputContract
{
    public record PartialPair
    {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; init; }

        [JsonPropertyName("candles")]
        public IEnumerable<Candle> Candles { get; init; } = [];
    }
}
