using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.StockFearAndGreedContracts
{
    public class StockFearAndGreedData
    {
        [JsonPropertyName("now")]
        public Now Now { get; set; }
    }

    public class Now
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("valueText")]
        public string ValueText { get; set; }
    }
}
