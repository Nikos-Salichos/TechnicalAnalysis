using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class StPatternSignal : BaseIndicator
    {
        public bool IsBuy { get; init; }
        public bool IsSell { get; init; }
        public StPatternSignal(long candlestickId) : base(candlestickId)
        {
        }
    }
}
