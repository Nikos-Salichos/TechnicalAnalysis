namespace TechnicalAnalysis.Domain.Utilities
{
    public static class ParallelConfig
    {
        public static ParallelOptions GetOptions()
            => new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount / 4
            };
    }
}
