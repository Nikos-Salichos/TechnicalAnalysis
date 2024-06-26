using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.Binance
{
    public class BinanceSymbol
    {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; init; }

        [JsonPropertyName("baseAsset")]
        public string? BaseAsset { get; init; }

        [JsonPropertyName("quoteAsset")]
        public string? QuoteAsset { get; init; }

        [JsonPropertyName("status")]
        public string? Status { get; init; }
    }
}
