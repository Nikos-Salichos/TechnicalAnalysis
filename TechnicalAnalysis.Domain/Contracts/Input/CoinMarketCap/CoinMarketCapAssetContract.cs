using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CoinMarketCap
{
    public record CoinMarketCapAssetContract(
        [property: JsonPropertyName("data")] List<Data> Data,
        [property: JsonPropertyName("status")] Status Status
    );

    public record Data(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("symbol")] string? Symbol,
        [property: JsonPropertyName("slug")] string? Slug,
        [property: JsonPropertyName("num_market_pairs")] int NumMarketPairs,
        [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
        [property: JsonPropertyName("date_added")] DateTime DateAdded
    );

    public record Status(
        [property: JsonPropertyName("timestamp")] DateTime Timestamp,
        [property: JsonPropertyName("error_code")] int ErrorCode,
        [property: JsonPropertyName("error_message")] string? ErrorMessage,
        [property: JsonPropertyName("elapsed")] int Elapsed,
        [property: JsonPropertyName("credit_count")] int CreditCount
    );

}
