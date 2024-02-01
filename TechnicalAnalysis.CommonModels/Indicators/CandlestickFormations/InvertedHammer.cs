using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class InvertedHammer : BaseIndicator
    {
        public bool IsInvertedHammer { get; init; }
        public int OrderOfSignal { get; init; }
        public InvertedHammer(long candlestickId, int orderOfSignal, bool isInvertedHammer)
            : base(candlestickId)
        {
            IsInvertedHammer = isInvertedHammer;
            OrderOfSignal = orderOfSignal;
        }

        public static InvertedHammer Create(long candlestickId, int orderOfSignal, bool isInvertedHammer)
        {
            return new InvertedHammer(candlestickId, orderOfSignal, isInvertedHammer);
        }
    }
}
