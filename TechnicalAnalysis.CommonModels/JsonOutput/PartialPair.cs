using System.Text.Json.Serialization;

namespace TechnicalAnalysis.CommonModels.JsonOutput
{
    public class PartialPair
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; init; }

        [JsonPropertyName("candles")]
        public IEnumerable<Candle> Candles { get; init; } = new List<Candle>();
    }
}
