using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Infrastructure.Persistence.DistributedCache;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;

namespace TechnicalAnalysis.Application.Services
{
    public class CachedAnalysisService(IAnalysisService inner, IDistributedCache distributedCache,
        ICommunication communication, IRabbitMqService rabbitMqService) : IAnalysisService
    {
        public Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(string pairName, Timeframe timeframe)
        {
            return inner.GetIndicatorsByPairNamesAsync(pairName, timeframe);
        }

        public async Task<IEnumerable<PairExtended>> GetPairsIndicatorsAsync(DataProvider provider = DataProvider.All, HttpContext? httpContext = null)
        {
            if (httpContext?.Request.Headers.ContainsKey("C-Invalid") == false)
            {
                var cachedPairs = await distributedCache.GetRecordAsync<IEnumerable<PairExtended>>(provider.ToString());
                if (cachedPairs is not null)
                {
                    await communication.CreateAttachmentSendMessage(cachedPairs);
                    rabbitMqService.PublishMessage(cachedPairs);
                    rabbitMqService.PublishMessage(cachedPairs);
                    var message = await rabbitMqService.ConsumeMessageAsync<IEnumerable<PairExtended>>();
                    return cachedPairs;
                }
            }

            var pairs = await inner.GetPairsIndicatorsAsync(provider, httpContext);

            await distributedCache.SetRecordAsync(provider.ToString(), pairs, null, null);
            await communication.CreateAttachmentSendMessage(pairs);
            rabbitMqService.PublishMessage(pairs);

            return pairs;
        }
    }
}
