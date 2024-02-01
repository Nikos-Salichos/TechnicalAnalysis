using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class WallStreetZenAdapter(ILogger<WallStreetZenAdapter> logger, IWallStreetZenClient wallStreetZenClient) : IAdapter
    {
        public Task Sync(DataProvider provider, Timeframe timeframe)
        {
            var stocks = wallStreetZenClient.Sync();
            return Task.CompletedTask;
        }
    }
}
