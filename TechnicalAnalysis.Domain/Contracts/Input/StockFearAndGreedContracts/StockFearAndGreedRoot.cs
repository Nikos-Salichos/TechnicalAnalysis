using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.StockFearAndGreedContracts
{
    public record StockFearAndGreedRoot(
        [property: JsonPropertyName("fgi")] StockFearAndGreedData StockFearAndGreedData,
        [property: JsonPropertyName("lastUpdated")] StockFearAndGreedLastUpdated StockFearAndGreedLastUpdated
    );

    public record StockFearAndGreedLastUpdated(
        [property: JsonPropertyName("epochUnixSeconds")] int EpochUnixSeconds,
        [property: JsonPropertyName("humanDate")] string? HumanDate
    );

    public record StockFearAndGreedData(
        [property: JsonPropertyName("now")] Now Now
    );

    public record Now(
        [property: JsonPropertyName("value")] int Value,
        [property: JsonPropertyName("valueText")] string ValueText
    );
}
