using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Entities
{
    public class DexCandlestick : BaseEntity
    {
        public string? PoolContract { get; init; }
        public long PoolId { get; init; }
        public decimal? OpenPrice { get; init; }
        public decimal? HighPrice { get; init; }
        public decimal? LowPrice { get; init; }
        public decimal? ClosePrice { get; init; }
        public Timeframe Timeframe { get; init; }
        public DateTime OpenDate { get; init; }
        public DateTime CloseDate
        {
            get
            {
                DateTime openDateUtc = OpenDate.ToUniversalTime();
                return openDateUtc.Date.AddHours(23).AddMinutes(59);
            }
        }
        public long? NumberOfTrades { get; init; }
        public long? Liquidity { get; init; }
        public decimal? Fees { get; init; }
        public decimal? TotalValueLocked { get; init; }
        public decimal? Volume { get; init; }
    }
}
