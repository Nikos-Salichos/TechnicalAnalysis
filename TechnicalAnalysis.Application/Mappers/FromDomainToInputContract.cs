using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;

namespace TechnicalAnalysis.Application.Mappers
{
    public static class FromDomainToInputContract
    {
        private static BinanceCandlestick ToContract(this CandlestickExtended candlestick)
            => new()
            {
                OpenPrice = candlestick.OpenPrice,
                ClosePrice = candlestick.ClosePrice,
                HighPrice = candlestick.HighPrice,
                LowPrice = candlestick.LowPrice,
                OpenTime = candlestick.OpenDate,
                CloseTime = candlestick.CloseDate,
                Volume = candlestick.Volume,
                NumberOfTrades = candlestick.NumberOfTrades
            };

        public static IEnumerable<BinancePair> ToContract(this IEnumerable<PairExtended> pairs)
            => pairs.Select(p => p.ToContract()).ToList();

        private static BinancePair ToContract(this PairExtended pair)
            => new()
            {
                Id = pair.PrimaryId,
                Pair = pair.Symbol,
                BaseAssetId = pair.BaseAssetId,
                QuoteAssetId = pair.QuoteAssetId,
                Provider = pair.Provider,
                IsActive = pair.IsActive,
                AllCandles = pair.AllCandles,
                BinanceCandlesticks = pair.Candlesticks.ConvertAll(c => c.ToContract())
            };

        private static BinanceAsset ToContract(this Asset asset)
            => new()
            {
                Id = asset.PrimaryId,
                Asset = asset.Symbol
            };

        public static IEnumerable<BinanceAsset> ToContract(this IEnumerable<Asset> assets)
            => assets.Select(p => p.ToContract()).ToList();
    }
}
