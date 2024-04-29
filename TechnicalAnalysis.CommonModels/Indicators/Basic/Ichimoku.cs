using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Ichimoku(long candlestickId) : BaseIndicator(candlestickId)
    {
        public required decimal? TenkanSen { get; init; } // conversion line
        public required decimal? KijunSen { get; init; } // base line
        public required decimal? SenkouSpanA { get; init; } // leading span A
        public required decimal? SenkouSpanB { get; init; } // leading span B
        public required decimal? ChikouSpan { get; init; } // lagging span
    }
}
