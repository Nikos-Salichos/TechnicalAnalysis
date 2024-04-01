using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Output
{
    public class EnhancedScanGroup
    {
        public DateTime CandlestickCloseDate { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? OrderOfLongSignal { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? OrderOfShortSignal { get; init; }

        public decimal? CandlestickClosePrice { get; init; }

        public decimal? PercentageFromAllTimeHigh { get; init; }

        public decimal? DaysFromAllTimeHigh { get; init; }
    }
}
