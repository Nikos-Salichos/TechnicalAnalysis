using Microsoft.AspNetCore.Http;
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
        public async Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(IEnumerable<string> pairNames, Timeframe timeframe, HttpContext? httpContext = null)
        {
            var pairsFromCache = new List<PairExtended>();

            if (httpContext?.Request.Headers.ContainsKey("C-Invalid") == false)
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

            if (pairNames.Count() == pairsFromCache.Count)
            {
                return pairsFromCache;
            }

            var fetchedPairs = await inner.GetIndicatorsByPairNamesAsync(pairNames, timeframe);

            foreach (var pair in fetchedPairs)
            {
                await redisRepository.SetRecordAsync(pair.Symbol, pair, null, null);
                pairsFromCache.Add(pair);
            }

            return pairsFromCache;
        }

        public async Task<List<EnhancedPairResult>> GetEnhancedPairResultsAsync(DataProvider provider, HttpContext? httpContext = null)
        {
            var fileName = $"All-Pairs-Indicators-{DateTime.UtcNow}";

            if (httpContext?.Request.Headers.ContainsKey("C-Invalid") == false)
            {
                var cachedPairs = await redisRepository.GetRecordAsync<List<EnhancedPairResult>>(provider.ToString());
                if (cachedPairs?.Count > 0)
                {
                    await communication.CreateAttachmentSendMessage(cachedPairs, fileName);
                    rabbitMqService.PublishMessage(cachedPairs);

                    // Example how to consume message
                    // var message = await rabbitMqService.ConsumeMessageAsync<IEnumerable<PairExtended>>();

                    return cachedPairs;
                }
            }

            var pairs = await inner.GetEnhancedPairResultsAsync(provider, httpContext);

            await redisRepository.SetRecordAsync(provider.ToString(), pairs, null, null);
            await communication.CreateAttachmentSendMessage(pairs, fileName);
            rabbitMqService.PublishMessage(pairs);

            return pairs;
        }

        public Task<List<AssetRanking>> GetLayerOneAssets() => inner.GetLayerOneAssets();
    }
}
