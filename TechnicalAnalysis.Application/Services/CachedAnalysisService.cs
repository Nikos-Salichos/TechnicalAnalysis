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

        //TODO fix it to return proper type
        public async Task<IEnumerable<PairExtended>> GetPairsIndicatorsAsync(DataProvider provider, HttpContext? httpContext = null)
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

                    return [];
                }
            }

            var pairs = await inner.GetPairsIndicatorsAsync(provider, httpContext);

            var filteredPairs = pairs
               .OrderByDescending(pair => pair.CreatedAt)
                 .Select(pair =>
                 {
                     var enhancedScans = pair.Candlesticks
                         .Where(c => c.EnhancedScans.Count > 0)
                         .OrderByDescending(c => c.CloseDate)
                         .ThenBy(c => c.EnhancedScans.OrderBy(es => es.OrderOfSignal))
                         .GroupBy(c => c.PoolOrPairId)
                         .Select(group => new EnhancedScanGroup
                         {
                             CandlestickCloseDate = group.First().CloseDate,
                             EnhancedScans = group.First().EnhancedScans.ToList()
                         })
                         .ToList();

                     return new EnhancedPairResult
                     {
                         Symbol = pair.Symbol,
                         EnhancedScans = enhancedScans,
                         OrderOfSignal = enhancedScans[0].EnhancedScans[0].OrderOfSignal
                     };
                 })
                 .Where(result => result.EnhancedScans.Count > 0)
                 .ToList();

            var sortedPairsByEnhanced = filteredPairs
                .OrderByDescending(result => result.EnhancedScans.FirstOrDefault()?.CandlestickCloseDate)
                .ThenBy(result => result.OrderOfSignal)
                .ToList();

            //TODO Instead of anonymous object create a class
            await redisRepository.SetRecordAsync(provider.ToString(), sortedPairsByEnhanced, null, null);
            await communication.CreateAttachmentSendMessage(sortedPairsByEnhanced, fileName);
            rabbitMqService.PublishMessage(sortedPairsByEnhanced);

            return [];
        }
    }
}
