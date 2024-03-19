using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.Text.Json;
using System.Web;
using TechnicalAnalysis.Domain.Contracts.Input.CoinRanking;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class CoinRankingHttpClient(IOptionsMonitor<CoinRankingSetting> settings, IHttpClientFactory httpClientFactory,
        ILogger<CoinRankingHttpClient> logger, IPollyPolicy pollyPolicy) : ICoinRankingHttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
        private readonly IAsyncPolicy<HttpResponseMessage> _pollyPolicy = pollyPolicy.CreatePolicies<HttpResponseMessage>(3, TimeSpan.FromMinutes(5));

        public async Task<IResult<CoinRankingAssetContract, string>> SyncAssets(int offset)
        {
            _httpClient.DefaultRequestHeaders.Add("x-access-token", settings.CurrentValue.ApiKey);

            UriBuilder uriBuilder = new(settings.CurrentValue.ListingsLatestEndpoint);
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["tags"] = "layer-1";
            queryString["orderBy"] = "listedAt";
            queryString["limit"] = "100";
            queryString["offset"] = offset.ToString();

            uriBuilder.Query = queryString.ToString();

            using var httpResponseMessage = await _pollyPolicy.ExecuteAsync(() => _httpClient.GetAsync(uriBuilder.ToString(),
                HttpCompletionOption.ResponseHeadersRead));

            logger.LogInformation("SymbolsPairsPath {baseUrl}, httpResponseMessage '{@httpResponseMessage}' ",
                settings.CurrentValue.ListingsLatestEndpoint, httpResponseMessage);

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logger.LogError("{httpResponseMessage.StatusCode}", httpResponseMessage.StatusCode);
                return Result<CoinRankingAssetContract, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
            }

            try
            {
                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<CoinRankingAssetContract>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    logger.LogInformation("deserializedData '{@deserializedData}' ", deserializedData);
                    return Result<CoinRankingAssetContract, string>.Success(deserializedData);
                }

                logger.LogWarning("Deserialization Failed");
                return Result<CoinRankingAssetContract, string>.Fail($"{nameof(SyncAssets)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("{exception}", exception);
                return Result<CoinRankingAssetContract, string>.Fail(exception.ToString());
            }
        }
    }
}
