using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class DragonFlyDoji(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required bool IsDragonFlyDoji { get; init; }
        public int OrderOfSignal { get; init; }
    }
}
