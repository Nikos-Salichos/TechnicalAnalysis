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

    public class BinanceSymbolComparer : IEqualityComparer<BinanceSymbol>
    {
        public bool Equals(BinanceSymbol? x, BinanceSymbol? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Symbol == y.Symbol;
        }

        public int GetHashCode(BinanceSymbol obj)
        {
            return obj.Symbol.GetHashCode();
        }
    }
}
