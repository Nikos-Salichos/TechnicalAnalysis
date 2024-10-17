using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class FredApiAdapter(ILogger<FredApiAdapter> logger, IMediator mediator, IFredApiClient fredApiHttpClient) : IAdapter
    {
        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var result = await fredApiHttpClient.SyncVix();

            if (result.HasError)
            {
                logger.LogError("{DataProvider} initial response error {FailValue}", nameof(provider), result.FailValue);
                return false;
            }

            return true;
        }
    }
}
