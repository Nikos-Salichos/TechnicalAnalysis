using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.IO.Compression;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.Cnn;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class CnnStockFearAndGreedClient(IOptionsMonitor<CnnApiSetting> apiSetting, IHttpClientFactory httpClientFactory,
        ILogger<CnnStockFearAndGreedClient> logger, IPollyPolicy pollyPolicy) : ICnnStockFearAndGreedClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("cnn");
        private readonly ResiliencePipeline _resiliencePipeline = pollyPolicy.CreatePolicies(retries: 3);

        public async Task<Result<RootStockFearAndGreed, string>> GetCnnStockFearAndGreedIndex()
        {
            try
            {
                using var httpResponseMessage = await _resiliencePipeline.ExecuteAsync(async (ctx)
                    => await _httpClient.GetAsync(apiSetting.CurrentValue.StockFearAndGreedUri, HttpCompletionOption.ResponseHeadersRead, ctx));

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogError("{HttpResponseMessageStatusCode}", httpResponseMessage.StatusCode);
                    return Result<RootStockFearAndGreed, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
                }

                using var content = httpResponseMessage.Content;

                if (content.Headers.ContentEncoding.Contains("gzip"))
                {
                    await using var decompressedStream = new GZipStream(await content.ReadAsStreamAsync(), CompressionMode.Decompress);
                    var deserializedData = await JsonSerializer.DeserializeAsync<RootStockFearAndGreed>(decompressedStream, JsonHelper.JsonSerializerOptions);
                    if (deserializedData is not null)
                    {
                        return Result<RootStockFearAndGreed, string>.Success(deserializedData);
                    }
                }
                else
                {
                    logger.LogError("Http response headers do not contain gzip content enconding");
                }

                logger.LogError("RootStockFearAndGreed Failed");
                return Result<RootStockFearAndGreed, string>.Fail($"{nameof(GetCnnStockFearAndGreedIndex)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("Method {Method}, Exception {Exception} ", nameof(GetCnnStockFearAndGreedIndex), exception);
                return Result<RootStockFearAndGreed, string>.Fail(exception.Message);
            }
        }
    }
}
