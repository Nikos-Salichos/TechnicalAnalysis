using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.Domain.Interfaces.Application;

namespace TechnicalAnalysis.Infrastructure.Host.HostedServices
{
    public class SynchronizeProvidersService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SynchronizeProvidersService> _logger;
        private readonly List<(CommonModels.Enums.DataProvider Provider, CommonModels.Enums.Timeframe Timeframe, TimeSpan Interval)> _tasks;

        public SynchronizeProvidersService(IServiceProvider serviceProvider, ILogger<SynchronizeProvidersService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            _tasks = new List<(CommonModels.Enums.DataProvider, CommonModels.Enums.Timeframe, TimeSpan)>
            {
                (CommonModels.Enums.DataProvider.Binance, CommonModels.Enums.Timeframe.Daily, TimeSpan.FromMinutes(30)),
                (CommonModels.Enums.DataProvider.Alpaca, CommonModels.Enums.Timeframe.Daily, TimeSpan.FromMinutes(30)),
                (CommonModels.Enums.DataProvider.AlternativeMeCryptoFearAndGreedProvider, CommonModels.Enums.Timeframe.Daily, TimeSpan.FromMinutes(30)),
                (CommonModels.Enums.DataProvider.CnnApiStockFearAndGreedProvider, CommonModels.Enums.Timeframe.Daily, TimeSpan.FromMinutes(30)),
                (CommonModels.Enums.DataProvider.CoinPaprika, CommonModels.Enums.Timeframe.Daily, TimeSpan.FromDays(1)),
                (CommonModels.Enums.DataProvider.CoinMarketCap, CommonModels.Enums.Timeframe.Daily, TimeSpan.FromDays(1)),
                (CommonModels.Enums.DataProvider.CoinRanking, CommonModels.Enums.Timeframe.Daily, TimeSpan.FromDays(1))
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var syncService = scope.ServiceProvider.GetService<ISyncService>();

            if (syncService == null)
            {
                _logger.LogError("ISyncService is not registered.");
                return;
            }

            await InitialSynchronization(syncService);

            foreach (var (Provider, Timeframe, Interval) in _tasks)
            {
                _ = RunRecurringTask(Provider, Timeframe, Interval, stoppingToken);
            }
        }

        private async Task InitialSynchronization(ISyncService syncService)
        {
            try
            {
                var tasks = _tasks.Select(task =>
                    syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(task.Provider, task.Timeframe))
                );

                await Task.WhenAll(tasks);
                _logger.LogInformation("Initial synchronization completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during initial synchronization.");
            }
        }

        private async Task RunRecurringTask(CommonModels.Enums.DataProvider provider, CommonModels.Enums.Timeframe timeframe, TimeSpan interval, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var syncService = scope.ServiceProvider.GetService<ISyncService>();

                    if (syncService != null)
                    {
                        await syncService.SynchronizeProvidersAsync(new DataProviderTimeframeRequest(provider, timeframe));
                        _logger.LogInformation($"Synchronized provider {provider}.");
                    }
                    else
                    {
                        _logger.LogError($"Failed to resolve ISyncService for provider {provider}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in recurring task for provider {provider}.");
                }

                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}
