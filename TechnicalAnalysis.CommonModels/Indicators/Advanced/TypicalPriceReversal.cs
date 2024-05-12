using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class TypicalPriceReversal(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool TypicalPriceReversalIsBuy { get; init; }
        public bool TypicalPriceReversalIsSell { get; init; }
        public required int OrderOfSignal { get; init; }
    }
}
