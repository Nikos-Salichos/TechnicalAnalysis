using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class Marubozu(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool IsMarubozu { get; init; }
        public int OrderOfSignal { get; init; }
    }
}
