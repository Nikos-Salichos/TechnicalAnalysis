using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class SpinningTop : BaseIndicator
    {
        public bool IsSpinningTop { get; init; }
        public int OrderOfSignal { get; init; }
        public SpinningTop(long candlestickId, bool isSpinningTop)
            : base(candlestickId)
        {
            IsSpinningTop = isSpinningTop;
        }

        public static SpinningTop Create(long candlestickId, bool isSpinningTop)
        {
            return new SpinningTop(candlestickId, isSpinningTop);
        }
    }
}
