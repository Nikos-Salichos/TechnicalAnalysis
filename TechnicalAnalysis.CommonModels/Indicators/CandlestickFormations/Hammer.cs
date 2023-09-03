using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class Hammer : BaseIndicator
    {
        public bool IsHammer { get; init; }
        public int OrderOfSignal { get; init; }

        public Hammer(long candlestickId, bool isHammer, int orderOfSignal)
            : base(candlestickId)
        {
            IsHammer = isHammer;
            OrderOfSignal = orderOfSignal;
        }

        public static Hammer Create(long candlestickId, bool isHammer, int orderOfSignal)
        {
            return new Hammer(candlestickId, isHammer, orderOfSignal);
        }
    }
}
