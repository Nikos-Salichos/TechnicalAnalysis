using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.StockFearAndGreedContracts
{
    public class StockFearAndGreedLastUpdated
    {
        [JsonPropertyName("epochUnixSeconds")]
        public int EpochUnixSeconds { get; set; }

        [JsonPropertyName("humanDate")]
        public string? HumanDate { get; set; }
    }
}
