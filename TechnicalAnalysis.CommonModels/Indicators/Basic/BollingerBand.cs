using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class BollingerBand(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required long Period { get; init; }
        public required int? StandardDeviation { get; init; } = 2;
        public required decimal? UpperBand { get; init; }
        public required decimal? MiddleBand { get; init; }
        public required decimal? LowerBand { get; init; }
        public required decimal? PercentageBandwidth { get; init; }
        public required decimal? Width { get; init; }
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
