using System.Text.Json.Serialization;
using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class EnhancedScan : BaseIndicator
    {
        public bool EnhancedScanIsBuy { get; init; }

        public bool EnhancedScanIsSell { get; init; }

        public int OrderOfSignal { get; init; }

        [JsonConstructor]
        public EnhancedScan(long candlestickId) : base(candlestickId)
        {
            CandlestickId = candlestickId;
        }
    }
}
