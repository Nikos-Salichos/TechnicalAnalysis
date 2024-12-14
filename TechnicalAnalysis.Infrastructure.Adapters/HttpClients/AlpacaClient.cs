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
    public class AlpacaClient(IOptionsMonitor<AlpacaSetting> alpacaSettings, ILogger<AlpacaClient> logger,
        IPollyPolicy pollyPolicy) : IAlpacaClient
    {
        private readonly ResiliencePipeline _resiliencePipeline = pollyPolicy.CreatePolicies(retries: 3);

        public async Task<Result<IMultiPage<IBar>, string>> GetAlpacaData(string pairName, DateTime fromDateTime, DateTime toDateTime, BarTimeFrame barTimeFrame)
        {
            try
            {
                logger.LogInformation("Method {Method}, pairName {PairName}, toDateTime {ToDateTime}, barTimeFrame {BarTimeFrame} ", nameof(GetAlpacaData), pairName, toDateTime, barTimeFrame);
                var alpacaDataClient = Environments.Paper.GetAlpacaDataClient(new SecretKey(alpacaSettings.CurrentValue.ApiKey, alpacaSettings.CurrentValue.ApiSecret));
                HistoricalBarsRequest historicalBarsRequest = new(pairName, fromDateTime, toDateTime, barTimeFrame)
                {
                    Adjustment = Adjustment.SplitsAndDividends
                };

                ResilienceContext resilienceContext = ResilienceContextPool.Shared.Get();
                resilienceContext.Properties.Set(new ResiliencePropertyKey<string>("PairName"), pairName);
                resilienceContext.Properties.Set(new ResiliencePropertyKey<string>("FromDateTime"), fromDateTime.ToString());
                resilienceContext.Properties.Set(new ResiliencePropertyKey<string>("ToDateTime"), toDateTime.ToString());
                resilienceContext.Properties.Set(new ResiliencePropertyKey<string>("BarTimeFrame"), barTimeFrame.ToString());

                var alpacaStockData = await _resiliencePipeline.ExecuteAsync(async (ctx)
                    => await alpacaDataClient.GetHistoricalBarsAsync(historicalBarsRequest), resilienceContext);

               // return Result<IMultiPage<IBar>, string>.Success(alpacaStockData);

                return Result.Success(alpacaStockData);
            }
            catch (Exception exception)
            {
                logger.LogError("Method {Method}, Exception {Exception} ", nameof(GetAlpacaData), exception);
                return Result<IMultiPage<IBar>, string>.Fail(exception.Message);
            }
        }
    }
}
