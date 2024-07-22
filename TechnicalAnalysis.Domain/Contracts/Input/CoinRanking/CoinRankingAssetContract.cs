using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CoinRanking
{
    public record Coin(
        [property: JsonPropertyName("symbol")] string Symbol,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("listedAt")] int ListedAt
    );

    public record Data(
        [property: JsonPropertyName("stats")] Stats Stats,
        [property: JsonPropertyName("coins")] List<Coin> Coins
    );

    public record CoinRankingAssetContract(
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("data")] Data Data
    );

    public record Stats(
        [property: JsonPropertyName("total")] int Total,
        [property: JsonPropertyName("totalCoins")] int TotalCoins,
        [property: JsonPropertyName("totalMarkets")] int TotalMarkets,
        [property: JsonPropertyName("totalMarketCap")] string TotalMarketCap,
        [property: JsonPropertyName("total24hVolume")] string Total24hVolume
    );
}
