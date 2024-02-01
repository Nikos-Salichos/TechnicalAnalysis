using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.DexV3
{
    public class DexV3ApiResponse
    {
        [JsonPropertyName("data")]
        public PoolResponse PoolResponse { get; init; } = new PoolResponse();
    }
}
