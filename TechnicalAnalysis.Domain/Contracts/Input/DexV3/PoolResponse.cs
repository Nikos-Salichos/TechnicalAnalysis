using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.DexV3
{
    public class PoolResponse
    {
        [JsonPropertyName("pools")]
        public List<Pool> Pools { get; init; } = new List<Pool>();
    }
}
