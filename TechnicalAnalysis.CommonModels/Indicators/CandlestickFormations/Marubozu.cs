using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class Marubozu(long candlestickId, bool isMarubozu, int orderOfSignal) : BaseIndicator(candlestickId)
    {
        public bool IsMarubozu { get; init; } = isMarubozu;
        public int OrderOfSignal { get; init; } = orderOfSignal;

        public static Marubozu Create(long candlestickId, bool isMarubozu, int orderOfSignal)
        {
            return new Marubozu(candlestickId, isMarubozu, orderOfSignal);
        }
    }
}
