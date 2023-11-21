using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class WallStreetZenAdapter(ILogger<WallStreetZenAdapter> logger, IWallStreetZenClient WallStreetZenClient) : IAdapter
    {
        public Task Sync(DataProvider provider, Timeframe timeframe)
        {
            var stocks = WallStreetZenClient.Sync();
            return Task.CompletedTask;
        }
    }
}
