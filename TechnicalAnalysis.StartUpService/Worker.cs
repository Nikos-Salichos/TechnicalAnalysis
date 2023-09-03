using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Application;

namespace TechnicalAnalysis.StartUpService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTime.Now);
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();
                        await syncService.SynchronizeProvidersAsync(Provider.All);
                        var analysisService = scope.ServiceProvider.GetRequiredService<IAnalysisService>();
                        await analysisService.GetPairsIndicatorsAsync(Provider.All);
                    }
                    _logger.LogInformation("Worker finished at: {time}", DateTime.Now);
                    await Task.Delay(10000, stoppingToken);
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical("{exception}", exception);
            }
        }
    }
}