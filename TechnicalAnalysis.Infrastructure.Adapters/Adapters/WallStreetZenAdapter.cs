using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class WallStreetZenAdapter : IAdapter
    {
        private readonly ILogger<WallStreetZenAdapter> _logger;
        private readonly IWallStreetZenClient _wallStreetZenClient;

        public WallStreetZenAdapter(ILogger<WallStreetZenAdapter> logger, IWallStreetZenClient WallStreetZenClient)
        {
            _logger = logger;
            _wallStreetZenClient = WallStreetZenClient;
        }

        public Task Sync(Provider provider)
        {
            var stocks = _wallStreetZenClient.Sync();
            return Task.CompletedTask;
        }
    }
}
