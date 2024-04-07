using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class InvertedHammer(long candlestickId, int orderOfSignal, bool isInvertedHammer) : BaseIndicator(candlestickId)
    {
        public bool IsInvertedHammer { get; init; } = isInvertedHammer;
        public int OrderOfSignal { get; init; } = orderOfSignal;

        public static InvertedHammer Create(long candlestickId, int orderOfSignal, bool isInvertedHammer)
        {
            return new InvertedHammer(candlestickId, orderOfSignal, isInvertedHammer);
        }
    }
}
