using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class Marubozu : BaseIndicator
    {
        public bool IsMarubozu { get; init; }
        public int OrderOfSignal { get; init; }

        public Marubozu(long candlestickId, bool isMarubozu, int orderOfSignal)
            : base(candlestickId)
        {
            IsMarubozu = isMarubozu;
            OrderOfSignal = orderOfSignal;
        }

        public static Marubozu Create(long candlestickId, bool isMarubozu, int orderOfSignal)
        {
            return new Marubozu(candlestickId, isMarubozu, orderOfSignal);
        }
    }
}
