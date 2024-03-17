using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CoinRanking
{
    public class Coin
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("listedAt")]
        public int ListedAt { get; init; }
    }

    public class Data
    {
        [JsonPropertyName("stats")]
        public Stats Stats { get; init; }

        [JsonPropertyName("coins")]
        public List<Coin> Coins { get; init; }
    }

    public class CoinRankingAssetContract
    {
        [JsonPropertyName("status")]
        public string Status { get; init; }

        [JsonPropertyName("data")]
        public Data Data { get; init; }
    }

    public class Stats
    {
        [JsonPropertyName("total")]
        public int Total { get; init; }

        [JsonPropertyName("totalCoins")]
        public int TotalCoins { get; init; }

        [JsonPropertyName("totalMarkets")]
        public int TotalMarkets { get; init; }

        [JsonPropertyName("totalMarketCap")]
        public string TotalMarketCap { get; init; }

        [JsonPropertyName("total24hVolume")]
        public string Total24hVolume { get; init; }
    }
}
