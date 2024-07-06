using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CoinPaprika
{
    public record CoinPaprikaAssetContract(
        [property: JsonPropertyName("id")] string? CoinPaprikaId,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("symbol")] string? Symbol,
        [property: JsonPropertyName("rank")] int Rank,
        [property: JsonPropertyName("is_new")] bool IsNew,
        [property: JsonPropertyName("is_active")] bool IsActive,
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("CreatedAt")] DateTime CreatedAt
    );
}
