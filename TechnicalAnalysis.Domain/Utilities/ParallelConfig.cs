namespace TechnicalAnalysis.Domain.Utilities
{
    public static class ParallelConfig
    {
        public static ParallelOptions GetOptions()
            => new()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 3
            };
    }
}
