using Microsoft.Extensions.Caching.Distributed;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Infrastructure.Persistence.DistributedCache;
using Provider = TechnicalAnalysis.CommonModels.Enums.Provider;

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

        public Task<IEnumerable<PairExtended>> GetIndicatorsByPairNamesAsync(string pairName)
        {
            return _inner.GetIndicatorsByPairNamesAsync(pairName);
        }

        public async Task<IEnumerable<PairExtended>> GetPairsIndicatorsAsync(Provider provider = Provider.All)
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

            var pairs = await _inner.GetPairsIndicatorsAsync(provider);

            await _distributedCache.SetRecordAsync(provider.ToString(), pairs);
            await _communication.CreateAttachmentSendMessage(pairs);
            _rabbitMqService.PublishMessage(pairs);

            return pairs;
        }
    }
}
