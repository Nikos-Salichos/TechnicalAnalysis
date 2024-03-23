using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Contracts.Input.StockFearAndGreedContracts;
using TechnicalAnalysis.Domain.Entities;
using Candlestick = TechnicalAnalysis.Domain.Entities.Candlestick;
using Pair = TechnicalAnalysis.Domain.Entities.Pair;

namespace TechnicalAnalysis.Application.Mappers
{
    public static class FromDomainToEntity
    {
        private static Pair ToEntity(this PairExtended pair)
            => new()
            {
                PrimaryId = pair.PrimaryId,
                Symbol = pair.Symbol,
                BaseAssetId = pair.BaseAssetId,
                QuoteAssetId = pair.QuoteAssetId,
                Provider = pair.Provider,
                IsActive = pair.IsActive,
                AllCandles = pair.AllCandles,
                CreatedAt = pair.CreatedAt
            };

        public static List<Pair> ToEntity(this IEnumerable<PairExtended> pairs)
            => pairs.Select(c => c.ToEntity()).ToList();

        private static Candlestick ToEntity(this CandlestickExtended candlestick)
            => new()
            {
                PairId = candlestick.PoolOrPairId,
                OpenPrice = candlestick.OpenPrice,
                HighPrice = candlestick.HighPrice,
                LowPrice = candlestick.LowPrice,
                ClosePrice = candlestick.ClosePrice,
                Volume = candlestick.Volume,
                Timeframe = candlestick.Timeframe,
                OpenDate = candlestick.OpenDate,
                CloseDate = candlestick.CloseDate,
                NumberOfTrades = candlestick.NumberOfTrades
            };

        public static List<Candlestick> ToEntity(this IEnumerable<CandlestickExtended> candlesticks)
            => candlesticks.Select(c => c.ToEntity()).ToList();

        private static DexCandlestick DexToEntityCandlestick(this CandlestickExtended candlestick)
            => new()
            {
                PoolId = candlestick.PoolOrPairId,
                PoolContract = candlestick.PoolOrPairName,
                OpenDate = candlestick.OpenDate,
                OpenPrice = candlestick.OpenPrice,
                HighPrice = candlestick.HighPrice,
                LowPrice = candlestick.LowPrice,
                ClosePrice = candlestick.ClosePrice,
                Timeframe = candlestick.Timeframe,
                Fees = candlestick.Fees,
                Liquidity = candlestick.Liquidity,
                TotalValueLocked = candlestick.TotalValueLockedUsd,
                Volume = candlestick.Volume,
                NumberOfTrades = candlestick.NumberOfTrades
            };

        public static List<DexCandlestick> DexToEntityCandlestick(this IEnumerable<CandlestickExtended> candlesticks)
            => candlesticks.Select(c => c.DexToEntityCandlestick()).ToList();

        private static PoolEntity DexToEntityToken(this PairExtended pair)
            => new()
            {
                PrimaryId = pair.PrimaryId,
                PoolContract = pair.ContractAddress,
                Token0Id = pair.BaseAssetId,
                Token0Contract = pair.BaseAssetContract,
                Token1Id = pair.QuoteAssetId,
                Token1Contract = pair.QuoteAssetContract,
                FeeTier = pair.FeeTier,
                Fees = pair.Fees,
                Liquidity = pair.Liquidity,
                TotalValueLocked = pair.TotalValueLocked,
                Volume = pair.Volume,
                NumberOfTrades = pair.NumberOfTrades,
                Provider = pair.Provider,
                IsActive = pair.IsActive
            };

        public static List<PoolEntity> DexToEntityPool(this IEnumerable<PairExtended> pairs)
            => pairs.Select(c => c.DexToEntityToken()).ToList();

        public static StockFearAndGreedDomain? ToDomain(this StockFearAndGreedRoot stockFearAndGreedRoot)
            => stockFearAndGreedRoot is null
                ? null
                : new()
                {
                    DateTime = DateTimeOffset.FromUnixTimeSeconds(stockFearAndGreedRoot.StockFearAndGreedLastUpdated.EpochUnixSeconds).UtcDateTime.Date,
                    Value = stockFearAndGreedRoot.StockFearAndGreedData.Now.Value.ToString(),
                    ValueClassification = stockFearAndGreedRoot.StockFearAndGreedData.Now.ValueText
                };
    }
}
