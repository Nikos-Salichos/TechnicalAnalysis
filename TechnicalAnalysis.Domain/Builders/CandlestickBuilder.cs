using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces;

namespace TechnicalAnalysis.Domain.Builders
{
    public class CandlestickBuilder : ICandlestickBuilder
    {
        private readonly CandlestickExtended _candlestick = new();

        public CandlestickExtended Build()
        {
            return _candlestick;
        }

        public ICandlestickBuilder WithId(long id)
        {
            _candlestick.PrimaryId = id;
            return this;
        }

        public ICandlestickBuilder WithPoolOrPairId(long pairId)
        {
            _candlestick.PoolOrPairId = pairId;
            return this;
        }

        public ICandlestickBuilder WithPoolOrPairName(string? poolOrPairName)
        {
            _candlestick.PoolOrPairName = poolOrPairName;
            return this;
        }

        public ICandlestickBuilder WithOpenPrice(decimal? openPrice)
        {
            _candlestick.OpenPrice = openPrice;
            return this;
        }

        public ICandlestickBuilder WithHighPrice(decimal? highPrice)
        {
            _candlestick.HighPrice = highPrice;
            return this;
        }

        public ICandlestickBuilder WithLowPrice(decimal? lowPrice)
        {
            _candlestick.LowPrice = lowPrice;
            return this;
        }

        public ICandlestickBuilder WithClosePrice(decimal? closePrice)
        {
            _candlestick.ClosePrice = closePrice;
            return this;
        }

        public ICandlestickBuilder WithVolume(decimal? volume)
        {
            _candlestick.Volume = volume;
            return this;
        }

        public ICandlestickBuilder WithTimeframe(Timeframe timeframe)
        {
            _candlestick.Timeframe = timeframe;
            return this;
        }

        public ICandlestickBuilder WithOpenDate(DateTime openDate)
        {
            _candlestick.OpenDate = openDate;
            return this;
        }

        public ICandlestickBuilder WithCloseDate(DateTime closeDate)
        {
            _candlestick.CloseDate = closeDate;
            return this;
        }

        public ICandlestickBuilder WithFees(decimal? fees)
        {
            _candlestick.Fees = fees;
            return this;
        }

        public ICandlestickBuilder WithLiquidity(long? liquidity)
        {
            _candlestick.Liquidity = liquidity;
            return this;
        }

        public ICandlestickBuilder WithNumberOfTrades(long? numberOfTrades)
        {
            _candlestick.NumberOfTrades = numberOfTrades;
            return this;
        }

        public ICandlestickBuilder WithTotalValueLockedUsd(decimal? totalValueLockedUsd)
        {
            _candlestick.TotalValueLockedUsd = totalValueLockedUsd;
            return this;
        }
    }
}
