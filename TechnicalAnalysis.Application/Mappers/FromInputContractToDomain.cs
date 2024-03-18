using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Builders;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;
using TechnicalAnalysis.Domain.Extensions;

namespace TechnicalAnalysis.Application.Mappers
{
    public static class FromInputContractToDomain
    {
        public static List<CandlestickExtended> ToDomain(this IEnumerable<BinanceCandlestick> binanceCandlesticks)
            => binanceCandlesticks.Select(c => c.ToDomain()).ToList();

        private static Asset ToDomain(this BinanceAsset binanceAsset)
            => new()
            {
                PrimaryId = binanceAsset.Id,
                Symbol = binanceAsset.Asset
            };

        public static List<Asset> ToDomain(this IEnumerable<BinanceAsset> binanceAssets)
            => binanceAssets.Select(c => c.ToDomain()).ToList();

        public static CandlestickExtended ToDomain(this BinanceCandlestick binanceCandlestick)
            => new CandlestickBuilder().WithPoolOrPairId(binanceCandlestick.PairId)
              .WithOpenPrice(binanceCandlestick.OpenPrice)
              .WithHighPrice(binanceCandlestick.HighPrice)
              .WithLowPrice(binanceCandlestick.LowPrice)
              .WithClosePrice(binanceCandlestick.ClosePrice)
              .WithVolume(binanceCandlestick.Volume)
              .WithTimeframe(binanceCandlestick.Period.ToTimeFrame())
              .WithOpenDate(binanceCandlestick.OpenTime)
              .WithCloseDate(binanceCandlestick.CloseTime)
              .Build();

        private static CandlestickExtended FromDexCandlesticksV3ToDomain(this Data dexCandlestick)
            => new CandlestickBuilder().WithOpenPrice(dexCandlestick.Open?.ReduceDigitsToFitDecimalLength())
                          .WithHighPrice(dexCandlestick.High?.ReduceDigitsToFitDecimalLength())
                          .WithLowPrice(dexCandlestick.Low?.ReduceDigitsToFitDecimalLength())
                          .WithClosePrice(dexCandlestick.Close?.ReduceDigitsToFitDecimalLength())
                          .WithVolume(dexCandlestick.VolumeRawData.ReduceDigitsToFitDecimalLength())
                          .WithOpenDate(DateTimeOffset.FromUnixTimeSeconds(dexCandlestick.Date).UtcDateTime)
                          .WithFees(dexCandlestick.FeesRawData.ReduceDigitsToFitDecimalLength())
                          .WithLiquidity(dexCandlestick.LiquidityRawData.ReduceDigitsToFitLongLength())
                          .WithNumberOfTrades(dexCandlestick.NumberOfTrades.ReduceDigitsToFitLongLength())
                          .WithTotalValueLockedUsd(dexCandlestick.TotalValueLockedRawData?.ReduceDigitsToFitDecimalLength())
                          .Build();

        public static List<CandlestickExtended> FromDexCandlesticksV3ToDomain(this IEnumerable<Data> dexCandlesticks)
            => dexCandlesticks.Select(c => c.FromDexCandlesticksV3ToDomain()).ToList();

        public static List<PairExtended> ToDomain(this IEnumerable<Pool> pools)
            => pools.Select(c => c.ToDomain()).ToList();

        private static PairExtended ToDomain(this Pool pool)
            => new()
            {
                Symbol = pool.PoolId,
                FeeTier = pool.FeeTier,
                BaseAssetName = pool.Token0.Symbol,
                BaseAssetContract = pool.Token0.TokenId,
                QuoteAssetName = pool.Token1.Symbol,
                QuoteAssetContract = pool.Token1.TokenId,
                ContractAddress = pool.PoolId,
                Fees = pool.FeesRawData.ReduceDigitsToFitDecimalLength(),
                Liquidity = pool.LiquidityRawData.ReduceDigitsToFitLongLength(),
                TotalValueLocked = pool.TotalValueLockedRawData.ReduceDigitsToFitDecimalLength(),
                Volume = pool.VolumeRawData.ReduceDigitsToFitDecimalLength(),
                NumberOfTrades = pool.NumberOfTrades.ReduceDigitsToFitLongLength(),
                Candlesticks = pool.PoolDayData.FromDexCandlesticksV3ToDomain().ToList()
            };
    }
}
