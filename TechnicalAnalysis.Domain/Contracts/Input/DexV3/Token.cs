using System.Text.Json.Serialization;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Contracts.Input.DexV3
{
    public class Token
    {
        [JsonIgnore]
        public long Id { get; init; }

        [JsonPropertyName("id")]
        public string? TokenId { get; init; }
        public Chain ChainId { get; init; }
        public string? Symbol { get; init; }
        public string? Name { get; init; }

        [JsonPropertyName("TokenDayData")]
        public List<Data> TokenDayData { get; init; } = [];
    }
}
