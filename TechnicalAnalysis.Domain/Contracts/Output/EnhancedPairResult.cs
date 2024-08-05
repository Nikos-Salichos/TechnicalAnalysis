using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Contracts.Output
{
    public class EnhancedPairResult
    {
        public string? Symbol { get; init; }

        public ProductType ProductType { get; init; }

        public List<EnhancedScanGroup> EnhancedScans { get; init; } = [];
    }
}
