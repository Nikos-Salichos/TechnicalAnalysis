namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class CustomCandlestickData
    {
        public required decimal OpenPrice { get; init; }
        public required decimal HighPrice { get; init; }
        public required decimal LowPrice { get; init; }
        public required decimal ClosePrice { get; init; }
        public required decimal Volume { get; init; }
        public required DateTime OpenDate { get; init; }
        public required DateTime CloseDate { get; init; }
    }
}
