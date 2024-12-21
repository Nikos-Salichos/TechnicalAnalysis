using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.StockFearAndGreedContracts;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class StockFearAndGreedClient(IOptionsMonitor<RapidApiSetting> rapidApiSetting, IHttpClientFactory httpClientFactory,
        ILogger<StockFearAndGreedClient> logger, IPollyPolicy pollyPolicy) : IStockFearAndGreedClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
        private readonly ResiliencePipeline _resiliencePipeline = pollyPolicy.CreatePolicies(retries: 3);

        public async Task<Result<StockFearAndGreedRoot, string>> GetStockFearAndGreedIndex()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", rapidApiSetting.CurrentValue.StockFearAndGreedApiKey);
                _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", rapidApiSetting.CurrentValue.StockFearAndGreedHost);

                using var httpResponseMessage = await _resiliencePipeline.ExecuteAsync(async (ctx)
                    => await _httpClient.GetAsync(rapidApiSetting.CurrentValue.StockFearAndGreedUri, HttpCompletionOption.ResponseHeadersRead, ctx));

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogError("{HttpResponseMessageStatusCode}", httpResponseMessage.StatusCode);
                    return Result<StockFearAndGreedRoot, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
                }

                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<StockFearAndGreedRoot>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<StockFearAndGreedRoot, string>.Success(deserializedData);
                }

                logger.LogError("Deserialization Failed");
                return Result<StockFearAndGreedRoot, string>.Fail($"{nameof(GetStockFearAndGreedIndex)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("Method {Method}, Exception {Exception} ", nameof(GetStockFearAndGreedIndex), exception);
                return Result<StockFearAndGreedRoot, string>.Fail(exception.Message);
            }
        }
    }
}
