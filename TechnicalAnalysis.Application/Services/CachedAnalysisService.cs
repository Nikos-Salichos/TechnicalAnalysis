using Microsoft.AspNetCore.Http;
using System.Threading.Tasks.Dataflow;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Output;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;

namespace TechnicalAnalysis.Application.Services
{
    public class CachedAnalysisService(IAnalysisService inner, IRedisRepository redisRepository,
        ICommunication communication, IRabbitMqService rabbitMqService) : IAnalysisService
    {
        private static readonly ExecutionDataflowBlockOptions ExecutionDataflowBlockOptions = new()
        {
            MaxDegreeOfParallelism = 3 * Environment.ProcessorCount,
            SingleProducerConstrained = true,
        };

        public async Task<List<PairExtended>> GetIndicatorsByPairNamesAsync(List<string> pairNames, Timeframe timeframe, HttpContext? httpContext = null)
        {
            var pairsFromCache = new List<PairExtended>();

            if (httpContext?.Request.Headers.TryGetValue("X-Cache-Refresh", out var headerValue) == false
                && string.Equals(headerValue, "false", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var pairName in pairNames)
                {
                    var cachedPair = await redisRepository.GetRecordAsync<PairExtended>(pairName);
                    if (cachedPair != null)
                    {
                        pairsFromCache.Add(cachedPair);
                    }
                }
            }

            if (pairNames.Count == pairsFromCache.Count && pairNames.Count > 0)
            {
                return pairsFromCache;
            }

            var fetchedPairs = await inner.GetIndicatorsByPairNamesAsync(pairNames, timeframe);

            var block = new ActionBlock<PairExtended>(async pair =>
            {
                await redisRepository.SetRecordAsync(pair.Symbol, pair);
                pairsFromCache.Add(pair);

            }, ExecutionDataflowBlockOptions);

            foreach (var pair in fetchedPairs)
            {
                block.Post(pair);
            }

            block.Complete();
            await block.Completion;

            return pairsFromCache;
        }

        public async Task<List<EnhancedPairResult>> GetEnhancedPairResultsAsync(DataProvider provider, HttpContext? httpContext = null)
        {
            var fileName = $"All-Pairs-Indicators-{DateTime.UtcNow}";

            if (httpContext?.Request.Headers.TryGetValue("X-Cache-Refresh", out var headerValue) == false
               && string.Equals(headerValue, "false", StringComparison.InvariantCultureIgnoreCase))
            {
                var cachedPairs = await redisRepository.GetRecordAsync<List<EnhancedPairResult>>(provider.ToString());
                if (cachedPairs?.Count > 0)
                {
                    await communication.CreateAttachmentSendMessage(cachedPairs, fileName);
                    rabbitMqService.PublishMessage(cachedPairs);

                    // Example how to consume message
                    // var message = await rabbitMqService.ConsumeMessageAsync<List<PairExtended>>();

                    return cachedPairs;
                }
            }

            var pairs = await inner.GetEnhancedPairResultsAsync(provider, httpContext);

            await redisRepository.SetRecordAsync(provider.ToString(), pairs, null, null);
            await communication.CreateAttachmentSendMessage(pairs, fileName);
            rabbitMqService.PublishMessage(pairs);

            return pairs;
        }

        public async Task<List<AssetRanking>> GetLayerOneAssetsAsync()
        {
            var layerOneAssets = await inner.GetLayerOneAssetsAsync();

            var fileName = $"New-Layer-One-{DateTime.UtcNow}";
            await communication.CreateAttachmentSendMessage(layerOneAssets, fileName);

            return layerOneAssets;
        }

        public Task<List<CandlestickExtended>> GetCustomCandlesticksAnalysisAsync(List<CustomCandlestickData> customCandlestickData)
            => inner.GetCustomCandlesticksAnalysisAsync(customCandlestickData);
    }
}
