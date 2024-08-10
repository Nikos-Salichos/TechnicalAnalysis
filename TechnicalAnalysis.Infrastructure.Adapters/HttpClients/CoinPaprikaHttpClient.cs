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
    public class CoinPaprikaHttpClient(IOptionsMonitor<CoinPaprikaSetting> coinPaprikaSetting, IHttpClientFactory httpClientFactory,
        ILogger<CoinPaprikaHttpClient> logger, IPollyPolicy pollyPolicy) : ICoinPaprikaHttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
        private readonly ResiliencePipeline _resiliencePipeline = pollyPolicy.CreatePolicies(retries: 3);

        public async Task<IResult<List<CoinPaprikaAssetContract>, string>> SyncAssets()
        {
            try
            {
                using var httpResponseMessage = await _resiliencePipeline.ExecuteAsync(async (ctx)
                    => await _httpClient.GetAsync(coinPaprikaSetting.CurrentValue.Endpoint, HttpCompletionOption.ResponseHeadersRead, ctx));

                logger.LogInformation("SymbolsPairsPath {BaseUrl}, HttpResponseMessage '{@httpResponseMessage}' ",
                    coinPaprikaSetting.CurrentValue.Endpoint, httpResponseMessage);

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogError("{HttpResponseMessageStatusCode}", httpResponseMessage.StatusCode);
                    return Result<List<CoinPaprikaAssetContract>, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
                }

                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<List<CoinPaprikaAssetContract>>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<List<CoinPaprikaAssetContract>, string>.Success(deserializedData);
                }

                logger.LogError("Deserialization Failed");
                return Result<List<CoinPaprikaAssetContract>, string>.Fail($"{nameof(SyncAssets)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("Method {Method}, Exception {Exception} ", nameof(SyncAssets), exception);
                return Result<List<CoinPaprikaAssetContract>, string>.Fail(exception.Message);
            }
        }
    }
}
