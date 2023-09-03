using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.Indicators.Basic
{
    public class Fractal : BaseIndicator
    {
        public FractalType FractalType { get; set; }
        public decimal? Value { get; set; }
        public long WindowPeriod { get; set; }
        public Fractal(long candlestickId) : base(candlestickId)
        {
        }
    }
}
