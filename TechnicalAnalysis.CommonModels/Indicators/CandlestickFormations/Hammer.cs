using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class Hammer(long candlestickId, bool isHammer, int orderOfSignal) : BaseIndicator(candlestickId)
    {
        public bool IsHammer { get; init; } = isHammer;
        public int OrderOfSignal { get; init; } = orderOfSignal;

        public static Hammer Create(long candlestickId, bool isHammer, int orderOfSignal)
        {
            return new Hammer(candlestickId, isHammer, orderOfSignal);
        }
    }
}
