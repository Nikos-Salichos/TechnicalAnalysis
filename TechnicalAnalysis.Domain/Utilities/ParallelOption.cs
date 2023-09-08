namespace TechnicalAnalysis.Domain.Utilities
{
    public static class ParallelOption
    {
        private static readonly ParallelOptions _options;

        static ParallelOption()
        {
            _options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount / 4
            };
        }

        public static ParallelOptions GetOptions()
        {
            return _options;
        }
    }
}
