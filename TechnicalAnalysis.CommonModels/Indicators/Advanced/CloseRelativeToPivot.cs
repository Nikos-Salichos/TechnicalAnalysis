using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class CloseRelativeToPivot : BaseIndicator
    {
        public long NumberOfConsecutiveCandlestickBelowPivot { get; init; }
        public long NumberOfConsecutiveCandlestickAbovePivot { get; init; }
        public PivotPoint PivotPoint { get; init; } = PivotPoint.Classic;

        public CloseRelativeToPivot(long candlestickId) : base(candlestickId)
        {
        }

    }
}
