using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class WallStreetZenAdapter(ILogger<WallStreetZenAdapter> logger, IWallStreetZenClient wallStreetZenClient) : IAdapter
    {
        public Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var stocks = wallStreetZenClient.Sync();
            return Task.FromResult(true);
        }
    }
}
