using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class BollingerBandsFunnel : BaseIndicator
    {
        public bool IsBollingerBandsFunnel { get; init; }

        public int NumberOfBollingerBandsFunnelCandlesticks { get; init; }

        public decimal? HighestPriceOfFunnel { get; init; }

        public BollingerBandsFunnel(long candlestickId) : base(candlestickId)
        {
        }

    }
}
