using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Advanced
{
    public class BollingerBandsFunnel(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required bool IsBollingerBandsFunnel { get; init; }

        public required int NumberOfBollingerBandsFunnelCandlesticks { get; init; }

        public required decimal? HighestPriceOfFunnel { get; init; }
    }
}
