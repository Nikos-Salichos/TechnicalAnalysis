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
        public Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(string pairName, Timeframe timeframe)
        {
            return inner.GetIndicatorsByPairNamesAsync(pairName, timeframe);
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

            await redisRepository.SetRecordAsync(provider.ToString(), pairs, null, null);
            await communication.CreateAttachmentSendMessage(pairs);
            rabbitMqService.PublishMessage(pairs);

            return pairs;
        }
    }
}
