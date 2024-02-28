using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.CoinPaprika;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class CoinPaprikaClient(IOptionsMonitor<BinanceSetting> binanceSettings, IHttpClientFactory httpClientFactory,
        ILogger<BinanceHttpClient> logger, IPollyPolicy pollyPolicy) : ICoinPaprikaClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");

        private readonly IAsyncPolicy<HttpResponseMessage> _pollyPolicy = pollyPolicy.CreatePolicies<HttpResponseMessage>(3, TimeSpan.FromMinutes(5));

        public async Task<IResult<IEnumerable<CoinPaprikaAsset>, string>> SyncAssets()
        {
            const string baseUrl = "https://api.coinpaprika.com/v1/coins";
            using var httpResponseMessage = await _pollyPolicy.ExecuteAsync(() => _httpClient.GetAsync(baseUrl, HttpCompletionOption.ResponseHeadersRead));

            logger.LogInformation("SymbolsPairsPath {baseUrl}, httpResponseMessage '{@httpResponseMessage}' ",
                binanceSettings.CurrentValue.SymbolsPairsPath, httpResponseMessage);

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logger.LogError("{httpResponseMessage.StatusCode}", httpResponseMessage.StatusCode);
                return Result<IEnumerable<CoinPaprikaAsset>, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
            }

            try
            {
                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<IEnumerable<CoinPaprikaAsset>>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    logger.LogInformation("deserializedData '{@deserializedData}' ", deserializedData);
                    return Result<IEnumerable<CoinPaprikaAsset>, string>.Success(deserializedData);
                }

                logger.LogWarning("Deserialization Failed");
                return Result<IEnumerable<CoinPaprikaAsset>, string>.Fail($"{nameof(SyncAssets)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("{exception}", exception);
                return Result<IEnumerable<CoinPaprikaAsset>, string>.Fail(exception.ToString());
            }

        }
    }
}
