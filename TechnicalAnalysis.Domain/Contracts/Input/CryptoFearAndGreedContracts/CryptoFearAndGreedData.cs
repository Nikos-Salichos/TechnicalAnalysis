using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts
{
    public class CryptoFearAndGreedData
    {
        [JsonPropertyName("value")]
        public string Value { get; init; } = string.Empty;

        [JsonPropertyName("value_classification")]
        public string ValueClassification { get; init; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; init; } = string.Empty;
    }
}
