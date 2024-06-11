namespace TechnicalAnalysis.Domain.Contracts.Output
{
    public class EnhancedPairResult
    {
        public string? Symbol { get; init; }
        public List<EnhancedScanGroup> EnhancedScans { get; init; } = [];
    }
}
