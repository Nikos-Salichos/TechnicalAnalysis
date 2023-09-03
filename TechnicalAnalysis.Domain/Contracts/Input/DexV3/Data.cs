using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.DexV3
{
    public class Data : BaseDexContract
    {
        [JsonPropertyName("Date")]
        public long Date { get; init; }

        [JsonPropertyName("Open")]
        public string? Open { get; init; }

        [JsonPropertyName("High")]
        public string? High { get; init; }

        [JsonPropertyName("Low")]
        public string? Low { get; init; }

        [JsonPropertyName("Close")]
        public string? Close { get; init; }

        [JsonPropertyName("tvlUSD")]
        public string? TotalValueLockedRawData { get; init; }
    }
}
