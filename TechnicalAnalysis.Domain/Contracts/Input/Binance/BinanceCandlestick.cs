using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.Binance
{
    public class BinanceCandlestick
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

        public string Period { get; set; } = string.Empty;

        public long PairId { get; set; }
    }
}