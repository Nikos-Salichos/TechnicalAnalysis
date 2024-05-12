using TechnicalAnalysis.CommonModels.BusinessModels;
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

        public static List<Pair> ToEntity(this List<PairExtended> pairs)
            => pairs.ConvertAll(c => c.ToEntity());

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

        public static List<Candlestick> ToEntity(this List<CandlestickExtended> candlesticks)
            => candlesticks.ConvertAll(c => c.ToEntity());

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

        public static List<DexCandlestick> DexToEntityCandlestick(this List<CandlestickExtended> candlesticks)
            => candlesticks.ConvertAll(c => c.DexToEntityCandlestick());

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

        public static List<PoolEntity> DexPoolToEntityPool(this List<PairExtended> pairs)
            => pairs.ConvertAll(c => c.DexToEntityToken());
    }
}
