using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class TypicalPriceReversal(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool TypicalPriceReversalIsBuy { get; init; }
        public bool TypicalPriceReversalIsSell { get; init; }
        public int OrderOfSignal { get; init; }
    }
}
