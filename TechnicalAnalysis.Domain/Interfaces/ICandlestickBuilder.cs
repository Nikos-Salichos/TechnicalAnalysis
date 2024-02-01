using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Interfaces
{
    public interface ICandlestickBuilder
    {
        ICandlestickBuilder WithId(long id);
        ICandlestickBuilder WithPoolOrPairId(long pairId);
        ICandlestickBuilder WithPoolOrPairName(string poolOrPairName);
        ICandlestickBuilder WithOpenPrice(decimal? openPrice);
        ICandlestickBuilder WithHighPrice(decimal? highPrice);
        ICandlestickBuilder WithLowPrice(decimal? lowPrice);
        ICandlestickBuilder WithClosePrice(decimal? closePrice);
        ICandlestickBuilder WithVolume(decimal? volume);
        ICandlestickBuilder WithTimeframe(Timeframe timeframe);
        ICandlestickBuilder WithOpenDate(DateTime openDate);
        ICandlestickBuilder WithCloseDate(DateTime closeDate);
        ICandlestickBuilder WithFees(decimal? fees);
        ICandlestickBuilder WithLiquidity(long? liquidity);
        ICandlestickBuilder WithTotalValueLockedUsd(decimal? totalValueLockedUsd);
        ICandlestickBuilder WithNumberOfTrades(long? numberOfTrades);
        CandlestickExtended Build();
    }
}
