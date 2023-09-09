using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;

namespace TechnicalAnalysis.Application.Mappers
{
    public static class FromDomainToInputContract
    {
        private static BinanceCandlestick ToContract(this CandlestickExtended candlestick)
        {
            return new BinanceCandlestick
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
        }

        public static IEnumerable<BinancePair> ToContract(this IEnumerable<PairExtended> pairs)
        {
            return pairs.Select(p => p.ToContract());
        }

        private static BinancePair ToContract(this PairExtended pair)
        {
            return new BinancePair
            {
                Id = pair.PrimaryId,
                Pair = pair.Symbol,
                BaseAssetId = pair.BaseAssetId,
                QuoteAssetId = pair.QuoteAssetId,
                Provider = pair.Provider,
                IsActive = pair.IsActive,
                AllCandles = pair.AllCandles,
                CreatedAt = pair.CreatedAt,
                BinanceCandlesticks = pair.Candlesticks.ConvertAll(c => c.ToContract())
            };
        }

        private static BinanceAsset ToContract(this Asset asset)
        {
            return new BinanceAsset
            {
                Id = asset.PrimaryId,
                Asset = asset.Symbol
            };
        }

        public static IEnumerable<BinanceAsset> ToContract(this IEnumerable<Asset> assets)
        {
            return assets.Select(p => p.ToContract());
        }
    }
}
