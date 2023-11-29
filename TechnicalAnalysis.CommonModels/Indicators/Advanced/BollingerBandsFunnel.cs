using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class BollingerBandsFunnel(long candlestickId) : BaseIndicator(candlestickId)
    {
        public bool IsBollingerBandsFunnel { get; init; }

        public int NumberOfBollingerBandsFunnelCandlesticks { get; init; }

        public decimal? HighestPriceOfFunnel { get; init; }
    }
}
