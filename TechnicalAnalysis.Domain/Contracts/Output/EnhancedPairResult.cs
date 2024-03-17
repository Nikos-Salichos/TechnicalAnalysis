namespace TechnicalAnalysis.Domain.Contracts.Output
{
    public class EnhancedPairResult
    {
        public string Symbol { get; init; } = string.Empty;
        public List<EnhancedScanGroup> EnhancedScans { get; init; } = [];
        public int OrderOfSignal { get; init; }
    }
}
