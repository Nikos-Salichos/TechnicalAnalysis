namespace TechnicalAnalysis.Domain.Contracts.Output
{
    public class EnhancedPairResult
    {
        public string Symbol { get; set; } = string.Empty;
        public List<EnhancedScanGroup> EnhancedScans { get; set; } = [];
        public int OrderOfSignal { get; set; }
    }
}
