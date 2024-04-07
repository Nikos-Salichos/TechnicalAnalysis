using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class BollingerBand(long candlestickId) : BaseIndicator(candlestickId)
    {
        public long Period { get; init; }
        public int? StandardDeviation { get; init; } = 2;
        public decimal? UpperBand { get; init; }
        public decimal? MiddleBand { get; init; }
        public decimal? LowerBand { get; init; }
        public decimal? PercentageBandwidth { get; init; }
        public decimal? Width { get; init; }
        public bool? OpenPriceOutOfBollinger { get; init; }
        public bool? WholeCandlestickOutOfBollinger { get; init; }
        public bool? LowPriceOutOfBollinger { get; init; }
        public bool? ClosePriceOutOfBollinger { get; init; }

        public decimal? BandWidth
        {
            get { return (UpperBand - LowerBand) / MiddleBand * 100; }
        }
    }
}
