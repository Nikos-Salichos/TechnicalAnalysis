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
        public async Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(string pairName, Timeframe timeframe)
        {
            var cachedPair = await redisRepository.GetRecordAsync<PairExtended>(pairName);
            if (cachedPair != null)
            {
                //  return new List<PairExtended> { cachedPair };
            }

            var pairs = await inner.GetIndicatorsByPairNamesAsync(pairName, timeframe);
            if (pairs.Any())
            {
                await redisRepository.SetRecordAsync(pairName, pairs.FirstOrDefault(), null, null);
            }

            return pairs;
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
                       EnhancedScans = enhancedScans
                   };
               })
               .Where(result => result.EnhancedScans.Count > 0)
               .ToList();

            var sortedPairs = filteredPairs.OrderByDescending(result => result.EnhancedScans.FirstOrDefault()?.CandlestickCloseDate).ToList();

            await redisRepository.SetRecordAsync(provider.ToString(), pairs, null, null);
            await communication.CreateAttachmentSendMessage(sortedPairs);
            rabbitMqService.PublishMessage(pairs);

            return pairs;
        }


    }
}
