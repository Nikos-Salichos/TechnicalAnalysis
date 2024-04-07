using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class DragonFlyDoji(long candlestickId, bool isDragonflyDoji, int orderOfSignal) : BaseIndicator(candlestickId)
    {
        public bool IsDragonFlyDoji { get; init; } = isDragonflyDoji;
        public int OrderOfSignal { get; init; } = orderOfSignal;

        public static DragonFlyDoji Create(long candlestickId, bool isDragonflyDoji, int orderOfSignal)
        {
            return new DragonFlyDoji(candlestickId, isDragonflyDoji, orderOfSignal);
        }
    }
}
