using Microsoft.AspNetCore.Http;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;

namespace TechnicalAnalysis.Application.Services
{
    public class CachedAnalysisService(IAnalysisService inner, IRedisRepository redisRepository,
        ICommunication communication, IRabbitMqService rabbitMqService) : IAnalysisService
    {
        public async Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(IEnumerable<string> pairNames, Timeframe timeframe)
        {
            var pairsFromCache = new List<PairExtended>();

            foreach (var pairName in pairNames)
            {
                var cachedPair = await redisRepository.GetRecordAsync<PairExtended>(pairName);
                if (cachedPair != null)
                {
                    pairsFromCache.Add(cachedPair);
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

        public async Task<IEnumerable<PairExtended>> GetPairsIndicatorsAsync(DataProvider provider, HttpContext? httpContext = null)
        {
            if (httpContext?.Request.Headers.ContainsKey("C-Invalid") == false)
            {
                var cachedPairs = await redisRepository.GetRecordAsync<IEnumerable<PairExtended>>(provider.ToString());
                if (cachedPairs?.Any() == true)
                {
                    await communication.CreateAttachmentSendMessage(cachedPairs);
                    rabbitMqService.PublishMessage(cachedPairs);

                    //Example how to consume message
                    var message = await rabbitMqService.ConsumeMessageAsync<IEnumerable<PairExtended>>();

                    return cachedPairs;
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
                         .Select(group => new
                         {
                             CandlestickCloseDate = group.First().CloseDate,
                             group.First().EnhancedScans
                         })
                         .ToList();

                     return new
                     {
                         pair.Symbol,
                         EnhancedScans = enhancedScans,
                         enhancedScans[0].EnhancedScans[0].OrderOfSignal
                     };
                 })
                 .Where(result => result.EnhancedScans.Count > 0)
                 .ToList();

            var sortedPairsByEnhanced = filteredPairs
                .OrderByDescending(result => result.EnhancedScans.FirstOrDefault()?.CandlestickCloseDate)
                .ThenBy(result => result.OrderOfSignal)
                .ToList();

            /*            var vhfPairs = pairs
                           .OrderByDescending(pair => pair.CreatedAt)
                           .Select(pair =>
                           {
                               var enhancedScans = pair.Candlesticks
                                   .Where(c => c.VerticalHorizontalFilterRanges.Count > 0)
                                   .OrderByDescending(c => c.CloseDate)
                                   .GroupBy(c => c.PoolOrPairId)
                                   .Select(group => new
                                   {
                                       CandlestickCloseDate = group.First().CloseDate,
                                       group.First().VerticalHorizontalFilterRanges
                                   })
                                   .ToList();

                               return new
                               {
                                   pair.Symbol,
                                   VerticalHorizontalFilterRanges = enhancedScans
                               };
                           })
                           .Where(result => result.VerticalHorizontalFilterRanges.Count > 0)
                           .ToList();

                        var sortedPairsByRange = vhfPairs.OrderByDescending(result => result.VerticalHorizontalFilterRanges.FirstOrDefault()?.CandlestickCloseDate).ToList();*/

            await redisRepository.SetRecordAsync(provider.ToString(), sortedPairsByEnhanced, null, null);
            await communication.CreateAttachmentSendMessage(sortedPairsByEnhanced);
            rabbitMqService.PublishMessage(pairs);

            return pairs;
        }
    }
}
