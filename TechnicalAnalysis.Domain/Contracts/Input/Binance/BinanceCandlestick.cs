using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.Binance
{
    public sealed class BinanceCandlestick : IEquatable<BinanceCandlestick>
    {
        [JsonPropertyOrder(1)]
        public DateTime OpenTime { get; init; }

        [JsonPropertyOrder(2)]
        public decimal? OpenPrice { get; init; }

        [JsonPropertyOrder(3)]
        public decimal? HighPrice { get; init; }

        [JsonPropertyOrder(4)]
        public decimal? LowPrice { get; init; }

        [JsonPropertyOrder(5)]
        public decimal? ClosePrice { get; init; }

        [JsonPropertyOrder(6)]
        [JsonIgnore]
        public decimal? Volume { get; init; }

        [JsonPropertyOrder(7)]
        public DateTime CloseTime { get; init; }

        [JsonPropertyOrder(8)]
        [JsonIgnore]
        public decimal QuoteAssetVolume { get; init; }

        [JsonPropertyOrder(9)]
        public long? NumberOfTrades { get; init; }

        public string? Period { get; set; }

        public long PairId { get; set; }

        public override bool Equals(object? obj) => Equals(obj as BinanceCandlestick);

        public bool Equals(BinanceCandlestick? other) => other != null
                && OpenTime == other.OpenTime
                && OpenPrice == other.OpenPrice
                && HighPrice == other.HighPrice
                && LowPrice == other.LowPrice
                && ClosePrice == other.ClosePrice
                && CloseTime == other.CloseTime
                && PairId == other.PairId;

        public override int GetHashCode() => HashCode.Combine(OpenTime, OpenPrice, HighPrice, LowPrice, ClosePrice, PairId);
    }
}