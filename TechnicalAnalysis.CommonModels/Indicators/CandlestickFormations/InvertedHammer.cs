using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class InvertedHammer(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required bool IsInvertedHammer { get; init; }
        public int OrderOfSignal { get; init; }
    }
}
