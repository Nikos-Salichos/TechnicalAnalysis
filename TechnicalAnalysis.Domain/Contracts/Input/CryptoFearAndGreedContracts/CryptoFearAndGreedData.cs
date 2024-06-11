using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts
{
    public class CryptoFearAndGreedData
    {
        [JsonPropertyName("value")]
        public string? Value { get; init; }

        [JsonPropertyName("value_classification")]
        public string? ValueClassification { get; init; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; init; }
    }
}
