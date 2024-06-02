using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.DexV3
{
    public abstract class BaseDexContract
    {
        [JsonPropertyName("feesUSD")]
        public string? FeesRawData { get; init; }

        [JsonPropertyName("volumeUsd")]
        public string? VolumeRawData { get; init; }

        [JsonPropertyName("liquidity")]
        public string? LiquidityRawData { get; init; }

        public virtual string? TotalValueLockedRawData { get; set; }

        [JsonPropertyName("txCount")]
        public string? NumberOfTrades { get; init; }
    }
}
