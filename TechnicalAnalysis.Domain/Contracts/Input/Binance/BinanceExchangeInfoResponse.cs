using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.Binance
{
    public class BinanceExchangeInfoResponse
    {
        [JsonPropertyName("timezone")]
        public string Timezone { get; init; } = string.Empty;

        [JsonPropertyName("serverTime")]
        public long ServerTime { get; init; }

        [JsonPropertyName("symbols")]
        public List<BinanceSymbol> Symbols { get; init; } = [];
    }
}
