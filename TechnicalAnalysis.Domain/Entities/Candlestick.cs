using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Entities
{
    public class Candlestick : BaseEntity
    {
        public long PairId { get; init; }
        public decimal? OpenPrice { get; init; }
        public decimal? HighPrice { get; init; }
        public decimal? LowPrice { get; init; }
        public decimal? ClosePrice { get; init; }
        public decimal? Volume { get; init; }
        public Timeframe Timeframe { get; init; }
        public DateTime OpenDate { get; init; }
        public DateTime CloseDate { get; init; }
        public long? NumberOfTrades { get; init; }
    }
}
