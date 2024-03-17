using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Fractal(long candlestickId) : BaseIndicator(candlestickId)
    {
        public FractalType FractalType { get; set; }
        public decimal? Value { get; set; }
        public long WindowPeriod { get; set; }
    }
}
