using System.Text.Json.Serialization;

namespace TechnicalAnalysis.CommonModels
{
    public class PartialPair
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; init; }

        [JsonPropertyName("candles")]
        public IEnumerable<Candle> Candles { get; init; } = new List<Candle>();
    }
}
