using Alpaca.Markets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class AlpacaHttpClient(IOptionsMonitor<AlpacaSetting> alpacaSettings, ILogger<AlpacaHttpClient> logger, IPollyPolicy pollyPolicy) : IAlpacaHttpClient
    {
        private readonly IAsyncPolicy<IMultiPage<IBar>> _retryPolicy = pollyPolicy.CreatePolicies<IMultiPage<IBar>>(3, TimeSpan.FromMinutes(5));

        public async Task<IResult<IMultiPage<IBar>, string>> GetAlpacaData(string pairName, DateTime fromDateTime, DateTime toDateTime, BarTimeFrame barTimeFrame)
        {
            logger.LogInformation("Method {Method}, pairName {pairName}, toDateTime {toDateTime}, barTimeFrame {barTimeFrame} ", nameof(GetAlpacaData), pairName, toDateTime, barTimeFrame);
            var alpacaDataClient = Environments.Paper.GetAlpacaDataClient(new SecretKey(alpacaSettings.CurrentValue.ApiKey, alpacaSettings.CurrentValue.ApiSecret));
            HistoricalBarsRequest historicalBarsRequest = new(pairName, fromDateTime, toDateTime, barTimeFrame)
            {
                Adjustment = Adjustment.SplitsAndDividends
            };
            var stockData = await _retryPolicy.ExecuteAsync(() => alpacaDataClient.GetHistoricalBarsAsync(historicalBarsRequest));
            return Result<IMultiPage<IBar>, string>.Success(stockData);
        }
    }
}
