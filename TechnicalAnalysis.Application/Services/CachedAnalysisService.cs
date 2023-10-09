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
    public class CachedAnalysisService : IAnalysisService
    {
        private readonly IAnalysisService _inner;
        private readonly IDistributedCache _distributedCache;
        private readonly ICommunication _communication;
        private readonly IRabbitMqService _rabbitMqService;

        public CachedAnalysisService(IAnalysisService inner, IDistributedCache distributedCache,
            ICommunication communication, IRabbitMqService rabbitMqService)
        {
            _inner = inner;
            _distributedCache = distributedCache;
            _communication = communication;
            _rabbitMqService = rabbitMqService;
        }

        public Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(string pairName, Timeframe timeframe)
        {
            return _inner.GetIndicatorsByPairNamesAsync(pairName, timeframe);
        }

        public async Task<IEnumerable<PairExtended>> GetPairsIndicatorsAsync(DataProvider provider = DataProvider.All, HttpContext? httpContext = null)
        {
            if (httpContext?.Request.Headers.ContainsKey("C-Invalid") == false)
            {
                var cachedPairs = await _distributedCache.GetRecordAsync<IEnumerable<PairExtended>>(provider.ToString());
                if (cachedPairs is not null)
                {
                    await _communication.CreateAttachmentSendMessage(cachedPairs);
                    _rabbitMqService.PublishMessage(cachedPairs);
                    _rabbitMqService.PublishMessage(cachedPairs);
                    var message = await _rabbitMqService.ConsumeMessageAsync<IEnumerable<PairExtended>>();
                    return cachedPairs;
                }
            }

            var pairs = await _inner.GetPairsIndicatorsAsync(provider, httpContext);

            await _distributedCache.SetRecordAsync(provider.ToString(), pairs, null, null);
            await _communication.CreateAttachmentSendMessage(pairs);
            _rabbitMqService.PublishMessage(pairs);

            return pairs;
        }
    }
}
