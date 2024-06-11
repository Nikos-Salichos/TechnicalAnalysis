using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CoinMarketCap
{
    public class CoinMarketCapAssetContract
    {
        [JsonPropertyName("data")]
        public List<Data> Data { get; init; }

        [JsonPropertyName("status")]
        public Status Status { get; init; }
    }

    public class Data
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; init; }

        [JsonPropertyName("slug")]
        public string? Slug { get; init; }

        [JsonPropertyName("num_market_pairs")]
        public int NumMarketPairs { get; init; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; init; }

        [JsonPropertyName("date_added")]
        public DateTime DateAdded { get; init; }
    }

    public class Status
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; init; }

        [JsonPropertyName("error_code")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; init; }

        [JsonPropertyName("elapsed")]
        public int Elapsed { get; init; }

        [JsonPropertyName("credit_count")]
        public int CreditCount { get; init; }
    }
}
