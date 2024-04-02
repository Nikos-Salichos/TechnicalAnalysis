using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mappers;
using TechnicalAnalysis.Application.Mediatr.Commands.Delete;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Builders;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;
using TechnicalAnalysis.Domain.Extensions;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class DexV3Adapter(IDexV3HttpClient dexV3HttpClient, ILogger<DexV3Adapter> logger,
         IMediator mediator) : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var dexV3Provider = exchanges.Find(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);

            dexV3Provider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            if (dexV3Provider.IsProviderCandlesticksSyncedToday(timeframe))
            {
                logger.LogInformation("{Provider} synchronized for today", provider);
                return true;
            }

            var pairs = await FormatDexAssetsPoolsCandlesticks(provider);
            var candlestickIds = pairs.SelectMany(p => p.Candlesticks.Select(c => c.PrimaryId)).ToList();
            var poolIds = pairs.Select(p => p.PrimaryId).ToList();

            var deleteCandlesticksResult = await mediator.Send(new DeleteDexCandlesticksCommand(candlestickIds));
            var poolsResult = await mediator.Send(new DeletePoolsCommand(poolIds));

            if (!deleteCandlesticksResult || !poolsResult)
            {
                return false;
            }

            const int numberOfPools = 10;
            const int numberOfData = 10;
            var apiResponse = await dexV3HttpClient.GetMostActivePoolsAsync(numberOfPools, numberOfData, provider);

            if (apiResponse.HasError)
            {
                logger.LogWarning("{apiResponse.IsError}", apiResponse.HasError);
                return false;
            }

            var poolsWithStables = FilterPoolsBasedOnStable(apiResponse.SuccessValue.PoolResponse);

            await SaveTokens(poolsWithStables);
            await SavePools(poolsWithStables, provider);
            await SaveCandlesticks(apiResponse.SuccessValue.PoolResponse, provider);

            dexV3Provider.ProviderPairAssetSyncInfo.UpdateProviderInfo();
            var providerCandlestickSyncInfo = dexV3Provider.GetOrCreateProviderCandlestickSyncInfo(provider, timeframe);
            await mediator.Send(new UpdateExchangeCommand(dexV3Provider.ProviderPairAssetSyncInfo, providerCandlestickSyncInfo));
            return true;
        }

        private async Task<IEnumerable<PairExtended>> FormatDexAssetsPoolsCandlesticks(DataProvider provider)
        {
            var fetchedAssetsTask = mediator.Send(new GetAssetsQuery());
            var fetchedPoolsTask = mediator.Send(new GetPoolsQuery());
            var fetchedCandlesticksTask = mediator.Send(new GetDexCandlesticksQuery());

            await Task.WhenAll(fetchedAssetsTask, fetchedPoolsTask, fetchedCandlesticksTask);

            var assetsResult = await fetchedAssetsTask;
            var poolsResult = await fetchedPoolsTask;
            var candlesticksResult = await fetchedCandlesticksTask;

            var pairs = poolsResult.Where(p => p.Provider == provider).PoolToDomain();
            var candlesticks = candlesticksResult.DexCandlestickToDomain();
            pairs.MapPairsToAssets(assetsResult);
            pairs.MapPairsToCandlesticks(candlesticks);

            return pairs;
        }

        private async Task SaveTokens(IEnumerable<Pool> pools)
        {
            var fetchedTokensResult = await mediator.Send(new GetAssetsQuery());

            HashSet<string> existingSymbols = new HashSet<string>(fetchedTokensResult.Select(t => t.Symbol));
            List<Asset> newAssets = new List<Asset>();

            foreach (var pool in pools)
            {
                if (!existingSymbols.Contains(pool.Token0.Symbol))
                {
                    existingSymbols.Add(pool.Token0.Symbol);
                    newAssets.Add(new Asset { Symbol = pool.Token0.Symbol });
                }

                if (!existingSymbols.Contains(pool.Token1.Symbol))
                {
                    existingSymbols.Add(pool.Token1.Symbol);
                    newAssets.Add(new Asset { Symbol = pool.Token1.Symbol });
                }
            }

            if (newAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsCommand(newAssets));
            }
        }

        private static IEnumerable<Pool> FilterPoolsBasedOnStable(PoolResponse poolResponse)
        {
            return poolResponse.Pools.Where(p =>
                p.Token0.Symbol == Constants.Usdt ||
                p.Token0.Symbol == Constants.Usdc ||
                p.Token0.Symbol == Constants.Busd ||
                p.Token0.Symbol == Constants.Dai ||
                p.Token1.Symbol == Constants.Usdt ||
                p.Token1.Symbol == Constants.Usdc ||
                p.Token1.Symbol == Constants.Busd ||
                p.Token1.Symbol == Constants.Dai
            );
        }

        private async Task SavePools(IEnumerable<Pool> pools, DataProvider provider)
        {
            List<PairExtended> newPairs = [];

            var fetchedAssetsTask = mediator.Send(new GetAssetsQuery());
            var fetchedPoolsTask = mediator.Send(new GetPoolsQuery());

            await Task.WhenAll(fetchedAssetsTask, fetchedPoolsTask);

            var tokens = await fetchedAssetsTask;
            var pairs = (await fetchedPoolsTask).PoolToDomain().Where(d => d.Provider == provider);

            foreach (var pair in pools.ToDomain())
            {
                pair.Provider = provider;

                var token0Id = tokens.FirstOrDefault(p => p.Symbol == pair.BaseAssetName);
                var token1Id = tokens.FirstOrDefault(p => p.Symbol == pair.QuoteAssetName);

                if (token0Id != null)
                {
                    pair.BaseAssetId = token0Id.PrimaryId;
                }

                if (token1Id != null)
                {
                    pair.QuoteAssetId = token1Id.PrimaryId;
                }

                pair.Symbol = pair.BaseAssetName + "-" + pair.QuoteAssetName;

                if (pair.BaseAssetId != 0 && pair.QuoteAssetId != 0)
                {
                    pair.IsActive = true;
                    newPairs.Add(pair);
                }
            }

            var pairsHashSet = new HashSet<(string poolContractAddress, DataProvider Provider)>(pairs.Select(p => (p.ContractAddress, p.Provider)));
            var uniquePairs = newPairs.Where(np => !pairsHashSet.Contains((np.ContractAddress, np.Provider))).ToList();

            await mediator.Send(new InsertPoolsCommand(uniquePairs.DexPoolToEntityPool()));
        }

        private async Task SaveCandlesticks(PoolResponse poolResponse, DataProvider provider)
        {
            var fetchedPoolsTask = await mediator.Send(new GetPoolsQuery());

            var pools = fetchedPoolsTask.Where(p => p.Provider == provider);

            List<CandlestickExtended> newCandlesticks = [];
            foreach (var pool in poolResponse.Pools)
            {
                var fetchedPoolFound = pools.FirstOrDefault(x => string.Equals(x.PoolContract, pool.PoolId));

                if (fetchedPoolFound == null)
                {
                    continue;
                }

                var firstTokenIsStable = false;

                if (pool.Token0.Symbol == Constants.Usdt ||
                    pool.Token0.Symbol == Constants.Usdc ||
                    pool.Token0.Symbol == Constants.Busd ||
                    pool.Token0.Symbol == Constants.Dai)
                {
                    foreach (var tokenDayData in pool.Token1.TokenDayData)
                    {
                        var candlestickBuilder = new CandlestickBuilder();

                        var openDate = DateTimeOffset.FromUnixTimeSeconds(tokenDayData.Date).UtcDateTime.Date;
                        var closeDate = openDate.Date.Add(new TimeSpan(23, 59, 59));

                        var candlestick = candlestickBuilder.WithPoolOrPairId(fetchedPoolFound.PrimaryId)
                                  .WithPoolOrPairName(pool.PoolId)
                                  .WithOpenPrice(tokenDayData?.Open?.ReduceDigitsToFitDecimalLength())
                                  .WithHighPrice(tokenDayData?.High?.ReduceDigitsToFitDecimalLength())
                                  .WithLowPrice(tokenDayData?.Low?.ReduceDigitsToFitDecimalLength())
                                  .WithClosePrice(tokenDayData?.Close?.ReduceDigitsToFitDecimalLength())
                                  .WithOpenDate(openDate)
                                  .WithCloseDate(closeDate)
                                  .WithTimeframe(Timeframe.Daily)
                                  .Build();

                        newCandlesticks.Add(candlestick);
                    }
                    firstTokenIsStable = true;
                }

                if (firstTokenIsStable)
                {
                    continue;
                }

                if (pool.Token1.Symbol == Constants.Usdt ||
                    pool.Token1.Symbol == Constants.Usdc ||
                    pool.Token1.Symbol == Constants.Busd ||
                    pool.Token1.Symbol == Constants.Dai)
                {
                    foreach (var tokenDayData in pool.Token0.TokenDayData)
                    {
                        var candlestickBuilder = new CandlestickBuilder();

                        var openDate = DateTimeOffset.FromUnixTimeSeconds(tokenDayData.Date).UtcDateTime.Date;
                        var closeDate = openDate.Date.Add(new TimeSpan(23, 59, 59));

                        var candlestick = candlestickBuilder.WithPoolOrPairId(fetchedPoolFound.PrimaryId)
                                  .WithPoolOrPairName(pool.PoolId)
                                  .WithOpenPrice(tokenDayData?.Open?.ReduceDigitsToFitDecimalLength())
                                  .WithHighPrice(tokenDayData?.High?.ReduceDigitsToFitDecimalLength())
                                  .WithLowPrice(tokenDayData?.Low?.ReduceDigitsToFitDecimalLength())
                                  .WithClosePrice(tokenDayData?.Close?.ReduceDigitsToFitDecimalLength())
                                  .WithOpenDate(openDate)
                                  .WithCloseDate(closeDate)
                                  .WithTimeframe(Timeframe.Daily)
                                  .Build();

                        newCandlesticks.Add(candlestick);
                    }
                }
            }

            await mediator.Send(new InsertDexCandlesticksCommand(newCandlesticks.DexToEntityCandlestick()));
        }
    }
}