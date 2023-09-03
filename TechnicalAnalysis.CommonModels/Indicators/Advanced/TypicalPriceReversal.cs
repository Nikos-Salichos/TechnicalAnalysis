using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class TypicalPriceReversal : BaseIndicator
    {
        public bool TypicalPriceReversalIsBuy { get; init; }
        public bool TypicalPriceReversalIsSell { get; init; }
        public int OrderOfSignal { get; init; }

        public TypicalPriceReversal(long candlestickId)
            : base(candlestickId)
        { }
    }
}
