using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.FredApiContracts;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class FredApiClient(IOptionsMonitor<FredApiSetting> fredApiSettings, IHttpClientFactory httpClientFactory,
        ILogger<FredApiClient> logger, IPollyPolicy pollyPolicy) : IFredApiClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
        private readonly ResiliencePipeline _resiliencePipeline = pollyPolicy.CreatePolicies(retries: 3);

        public async Task<Result<FredVixContract, string>> SyncVix()
        {
            try
            {
                using var httpResponseMessage = await _resiliencePipeline.ExecuteAsync(async (ctx)
                    => await _httpClient.GetAsync(fredApiSettings.CurrentValue.VixEndpoint, HttpCompletionOption.ResponseHeadersRead, ctx));

                logger.LogInformation("SymbolsPairsPath {BaseUrl}, HttpResponseMessage '{@httpResponseMessage}' ",
                    fredApiSettings.CurrentValue.VixEndpoint, httpResponseMessage);

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogError("{HttpResponseMessageStatusCode}", httpResponseMessage.StatusCode);
                    return Result<FredVixContract, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
                }

                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<FredVixContract>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<FredVixContract, string>.Success(deserializedData);
                }

                logger.LogError("Deserialization Failed");
                return Result<FredVixContract, string>.Fail($"{nameof(SyncVix)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("Method {Method}, Exception {Exception} ", nameof(SyncVix), exception);
                return Result<FredVixContract, string>.Fail(exception.Message);
            }
        }
    }
}
