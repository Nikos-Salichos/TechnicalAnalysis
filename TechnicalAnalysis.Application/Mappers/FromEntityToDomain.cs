﻿using TechnicalAnalysis.Domain.Builders;
using CandlestickExtended = TechnicalAnalysis.CommonModels.BusinessModels.CandlestickExtended;
using PairExtended = TechnicalAnalysis.CommonModels.BusinessModels.PairExtended;

namespace TechnicalAnalysis.Application.Mappers
{
    public static class FromEntityToDomain
    {
        private static PairExtended ToDomain(this Domain.Entities.Pair pair)
            => new()
            {
                PrimaryId = pair.PrimaryId,
                Symbol = pair.Symbol,
                BaseAssetId = pair.BaseAssetId,
                QuoteAssetId = pair.QuoteAssetId,
                Provider = pair.Provider,
                IsActive = pair.IsActive,
                AllCandles = pair.AllCandles,
                CreatedAt = pair.CreatedAt,
                Candlesticks = []
            };

        public static List<PairExtended> ToDomain(this List<Domain.Entities.Pair> pairs)
            => pairs.ConvertAll(c => c.ToDomain());

        private static CandlestickExtended ToDomain(this Domain.Entities.Candlestick candlestick)
            => new CandlestickBuilder().WithId(candlestick.PrimaryId)
                          .WithPoolOrPairId(candlestick.PairId)
                          .WithOpenPrice(candlestick.OpenPrice)
                          .WithHighPrice(candlestick.HighPrice)
                          .WithLowPrice(candlestick.LowPrice)
                          .WithClosePrice(candlestick.ClosePrice)
                          .WithVolume(candlestick.Volume)
                          .WithTimeframe(candlestick.Timeframe)
                          .WithOpenDate(candlestick.OpenDate)
                          .WithCloseDate(candlestick.CloseDate)
                          .Build();

        public static List<CandlestickExtended> ToDomain(this List<Domain.Entities.Candlestick> assets)
            => assets.ConvertAll(c => c.ToDomain());

        private static CandlestickExtended ToDomain(this Domain.Entities.DexCandlestick dexCandlestick)
            => new CandlestickBuilder().WithId(dexCandlestick.PrimaryId)
                          .WithPoolOrPairId(dexCandlestick.PoolId)
                          .WithPoolOrPairName(dexCandlestick.PoolContract)
                          .WithOpenDate(dexCandlestick.OpenDate)
                          .WithCloseDate(dexCandlestick.CloseDate)
                          .WithOpenPrice(dexCandlestick.OpenPrice)
                          .WithHighPrice(dexCandlestick.HighPrice)
                          .WithLowPrice(dexCandlestick.LowPrice)
                          .WithClosePrice(dexCandlestick.ClosePrice)
                          .WithTimeframe(dexCandlestick.Timeframe)
                          .WithFees(dexCandlestick.Fees)
                          .WithLiquidity(dexCandlestick.Liquidity)
                          .WithTotalValueLockedUsd(dexCandlestick.TotalValueLocked)
                          .WithVolume(dexCandlestick.Volume)
                          .WithNumberOfTrades(dexCandlestick.NumberOfTrades)
                          .Build();

        public static List<CandlestickExtended> DexCandlestickToDomain(this List<Domain.Entities.DexCandlestick> assets)
            => assets.ConvertAll(c => c.ToDomain());

        private static PairExtended PoolToDomain(this Domain.Entities.PoolEntity pool)
            => new()
            {
                PrimaryId = pool.PrimaryId,
                Provider = pool.Provider,
                ContractAddress = pool.PoolContract,
                BaseAssetId = pool.Token0Id,
                BaseAssetContract = pool.Token0Contract,
                QuoteAssetId = pool.Token1Id,
                QuoteAssetContract = pool.Token1Contract,
                FeeTier = pool.FeeTier,
                Fees = pool.Fees,
                Liquidity = pool.Liquidity,
                TotalValueLocked = pool.TotalValueLocked,
                Volume = pool.Volume,
                NumberOfTrades = pool.NumberOfTrades,
                IsActive = pool.IsActive,
                Candlesticks = []
            };

        public static List<PairExtended> PoolToDomain(this List<Domain.Entities.PoolEntity> pairs)
            => pairs.ConvertAll(c => c.PoolToDomain());
    }
}
