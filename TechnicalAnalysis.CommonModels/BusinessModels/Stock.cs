using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class Stock : BaseEntity
    {
        public string Symbol { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public decimal FairValuePrice { get; set; }
        public decimal UnderValuePercentage { get; set; }
        public decimal MinForecast { get; set; }
        public decimal MinForecastPercentage { get; set; }
        public decimal AvgForecast { get; set; }
        public decimal AvgForecastPercentage { get; set; }
        public decimal MaxForecast { get; set; }
        public decimal MaxForecastPercentage { get; set; }
    }
}
