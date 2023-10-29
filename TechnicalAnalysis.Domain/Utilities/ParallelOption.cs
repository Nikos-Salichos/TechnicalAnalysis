namespace TechnicalAnalysis.Domain.Utilities
{
    public static class ParallelOption
    {
        public static ParallelOptions GetOptions()
        {
            return new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount / 4
            };
        }
    }

}
