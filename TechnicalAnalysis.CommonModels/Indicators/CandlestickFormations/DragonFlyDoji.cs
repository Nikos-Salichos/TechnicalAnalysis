using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.CandlestickFormations
{
    public class DragonFlyDoji : BaseIndicator
    {
        public bool IsDragonFlyDoji { get; init; }
        public int OrderOfSignal { get; init; }
        public DragonFlyDoji(long candlestickId, bool isDragonflyDoji, int orderOfSignal)
            : base(candlestickId)
        {
            IsDragonFlyDoji = isDragonflyDoji;
            OrderOfSignal = orderOfSignal;
        }

        public static DragonFlyDoji Create(long candlestickId, bool isDragonflyDoji, int orderOfSignal)
        {
            return new DragonFlyDoji(candlestickId, isDragonflyDoji, orderOfSignal);
        }
    }
}
