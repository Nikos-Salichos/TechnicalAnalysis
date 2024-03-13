using Microsoft.Extensions.Logging;
using Polly;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.CoinPaprika;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class CoinPaprikaHttpClient(IHttpClientFactory httpClientFactory,
        ILogger<BinanceHttpClient> logger, IPollyPolicy pollyPolicy) : ICoinPaprikaHttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
        private readonly IAsyncPolicy<HttpResponseMessage> _pollyPolicy = pollyPolicy.CreatePolicies<HttpResponseMessage>(3, TimeSpan.FromMinutes(5));

        public async Task<IResult<IEnumerable<CoinPaprikaAssetContract>, string>> SyncAssets()
        {
            const string baseUrl = "https://api.coinpaprika.com/v1/coins";
            using var httpResponseMessage = await _pollyPolicy.ExecuteAsync(() => _httpClient.GetAsync(baseUrl, HttpCompletionOption.ResponseHeadersRead));

            logger.LogInformation("SymbolsPairsPath {baseUrl}, httpResponseMessage '{@httpResponseMessage}' ",
                baseUrl, httpResponseMessage);

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logger.LogError("{httpResponseMessage.StatusCode}", httpResponseMessage.StatusCode);
                return Result<IEnumerable<CoinPaprikaAssetContract>, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
            }

            try
            {
                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<IEnumerable<CoinPaprikaAssetContract>>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    logger.LogInformation("deserializedData '{@deserializedData}' ", deserializedData);
                    return Result<IEnumerable<CoinPaprikaAssetContract>, string>.Success(deserializedData);
                }

                logger.LogWarning("Deserialization Failed");
                return Result<IEnumerable<CoinPaprikaAssetContract>, string>.Fail($"{nameof(SyncAssets)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("{exception}", exception);
                return Result<IEnumerable<CoinPaprikaAssetContract>, string>.Fail(exception.ToString());
            }

        }
    }
}
