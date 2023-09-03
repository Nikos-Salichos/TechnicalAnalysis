using Alpaca.Markets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly.Timeout;
using TechnicalAnalysis.Domain.Interfaces;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class AlpacaHttpClient : IAlpacaHttpClient
    {
        private readonly IOptionsMonitor<AlpacaSetting> _alpacaSettings;
        private readonly ILogger<AlpacaHttpClient> _logger;

        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncTimeoutPolicy _asyncTimeoutPolicy;

        public AlpacaHttpClient(IOptionsMonitor<AlpacaSetting> alpacaSettings, ILogger<AlpacaHttpClient> logger, IPollyPolicy pollyPolicy)
        {
            _alpacaSettings = alpacaSettings;
            _logger = logger;
            _retryPolicy = pollyPolicy.CreateRetryPolicy(3, TimeSpan.FromSeconds(5));
            _asyncTimeoutPolicy = pollyPolicy.CreateTimeoutPolicy(TimeSpan.FromMinutes(5));
        }

        public async Task<IResult<IMultiPage<IBar>, string>> GetAlpacaData(string pairName, DateTime fromDateTime, DateTime toDateTime, BarTimeFrame barTimeFrame)
        {
            try
            {
                var alpacaDataClient = Environments.Paper.GetAlpacaDataClient(new SecretKey(_alpacaSettings.CurrentValue.ApiKey, _alpacaSettings.CurrentValue.ApiSecret));
                HistoricalBarsRequest historicalBarsRequest = new HistoricalBarsRequest(pairName, fromDateTime, toDateTime, barTimeFrame);
                var stockData = await _retryPolicy.WrapAsync(_asyncTimeoutPolicy).ExecuteAsync(() => alpacaDataClient.GetHistoricalBarsAsync(historicalBarsRequest));
                return Result<IMultiPage<IBar>, string>.Success(stockData);
            }
            catch (Exception exception)
            {
                _logger.LogCritical("{exception}", exception);
                return Result<IMultiPage<IBar>, string>.Fail(exception.ToString());
            }
        }

    }
}
