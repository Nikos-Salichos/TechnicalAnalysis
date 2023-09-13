using System.Text.Json.Serialization;

namespace TechnicalAnalysis.CommonModels.JsonOutput
{
    public class Signal
    {
        [JsonPropertyName("BUY")]
        public int Buy { get; set; }

        [JsonPropertyName("SELL")]
        public int Sell { get; set; }

        [JsonPropertyName("OPENED_AT")]
        public string OpenedAt { get; set; }
        public decimal? EntryPrice { get; init; }
    }
}
