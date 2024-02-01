using TechnicalAnalysis.CommonModels.BaseClasses;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class Asset : BaseEntity
    {
        public string Symbol { get; init; } = string.Empty;
        public DateTime CreatedDate { get; } = DateTime.UtcNow;
    }
}
