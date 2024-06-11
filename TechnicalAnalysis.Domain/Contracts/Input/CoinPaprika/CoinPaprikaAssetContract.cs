using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CoinPaprika
{
    public class CoinPaprikaAssetContract
    {
        [JsonPropertyName("id")]
        public string? CoinPaprikaId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("is_new")]
        public bool IsNew { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}
