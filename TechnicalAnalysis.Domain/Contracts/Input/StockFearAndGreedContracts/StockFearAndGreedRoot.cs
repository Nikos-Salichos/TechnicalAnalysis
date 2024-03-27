using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.StockFearAndGreedContracts
{
    public class StockFearAndGreedRoot
    {
        [JsonPropertyName("fgi")]
        public StockFearAndGreedData StockFearAndGreedData { get; init; }

        [JsonPropertyName("lastUpdated")]
        public StockFearAndGreedLastUpdated StockFearAndGreedLastUpdated { get; set; }
    }
}
