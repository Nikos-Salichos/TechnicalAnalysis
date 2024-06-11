using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.DexV3
{
    public class Pool : BaseDexContract
    {
        [JsonPropertyName("id")]
        public string? PoolId { get; init; }

        public string? FeeTier { get; init; }

        [JsonPropertyName("token0")]
        public Token Token0 { get; init; } = new Token();

        [JsonPropertyName("token1")]
        public Token Token1 { get; init; } = new Token();

        [JsonPropertyName("poolDayData")]
        public IEnumerable<Data> PoolDayData { get; init; } = [];

        [JsonPropertyName("totalValueLockedUSD")]
        public override string? TotalValueLockedRawData
        {
            get => base.TotalValueLockedRawData;
            set => base.TotalValueLockedRawData = value; // call the base setter to maintain the original logic
        }
    }
}
