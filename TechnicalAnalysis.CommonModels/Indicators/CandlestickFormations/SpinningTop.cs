using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class SpinningTop(long candlestickId, bool isSpinningTop) : BaseIndicator(candlestickId)
    {
        public bool IsSpinningTop { get; init; } = isSpinningTop;
        public int OrderOfSignal { get; init; }

        public static SpinningTop Create(long candlestickId, bool isSpinningTop)
        {
            return new SpinningTop(candlestickId, isSpinningTop);
        }
    }
}
