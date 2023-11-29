using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class CloseRelativeToPivot(long candlestickId) : BaseIndicator(candlestickId)
    {
        public long NumberOfConsecutiveCandlestickBelowPivot { get; init; }
        public long NumberOfConsecutiveCandlestickAbovePivot { get; init; }
        public PivotPoint PivotPoint { get; init; } = PivotPoint.Classic;
    }
}
