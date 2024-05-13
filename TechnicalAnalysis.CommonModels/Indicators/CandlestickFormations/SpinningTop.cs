using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class SpinningTop(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required bool IsSpinningTop { get; init; }
        public int OrderOfSignal { get; init; }
    }
}
