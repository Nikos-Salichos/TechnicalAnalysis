using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Ichimoku : BaseIndicator
    {
        public decimal? TenkanSen { get; init; } // conversion line
        public decimal? KijunSen { get; init; } // base line
        public decimal? SenkouSpanA { get; init; } // leading span A
        public decimal? SenkouSpanB { get; init; } // leading span B
        public decimal? ChikouSpan { get; init; } // lagging span
        public Ichimoku(long candlestickId) : base(candlestickId)
        {
        }
    }
}
