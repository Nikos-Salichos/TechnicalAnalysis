namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class StockFearAndGreedDomain
    {
        public string Value { get; init; } = string.Empty;

        public string ValueClassification { get; init; } = string.Empty;

        public DateTime DateTime { get; init; } = DateTime.UtcNow;
    }
}
