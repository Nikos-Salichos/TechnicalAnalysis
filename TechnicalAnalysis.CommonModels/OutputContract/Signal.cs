using System.Text.Json.Serialization;

namespace TechnicalAnalysis.CommonModels.OutputContract
{
    public class Signal
    {
        [JsonPropertyName("BUY")]
        public int Buy { get; set; }

        [JsonPropertyName("SELL")]
        public int Sell { get; set; }

        [JsonPropertyName("OPENED_AT")]
        public string OpenedAt { get; set; } = string.Empty;
        public decimal? EntryPrice { get; init; }
    }
}
