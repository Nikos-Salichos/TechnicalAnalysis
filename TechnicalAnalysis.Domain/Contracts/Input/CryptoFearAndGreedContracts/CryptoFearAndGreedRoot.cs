using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts
{
    public record CryptoFearAndGreedRoot(
        [property: JsonPropertyName("data")] IEnumerable<CryptoFearAndGreedData> CryptoFearAndGreedDatas
    );

    public record CryptoFearAndGreedData(
        [property: JsonPropertyName("value")] string? Value,
        [property: JsonPropertyName("value_classification")] string? ValueClassification,
        [property: JsonPropertyName("timestamp")] string? Timestamp
    );
}
