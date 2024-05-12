using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class Hammer(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool IsHammer { get; init; }
        public int OrderOfSignal { get; init; }
    }
}
