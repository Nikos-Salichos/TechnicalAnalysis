using Microsoft.Extensions.Logging;
using Polly;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class CryptoFearAndGreedHttpClient(IHttpClientFactory httpClientFactory, ILogger<CryptoFearAndGreedHttpClient> logger, IPollyPolicy pollyPolicy)
        : ICryptoFearAndGreedHttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");

        private readonly IAsyncPolicy<HttpResponseMessage> _pollyPolicy = pollyPolicy.CreatePolicies<HttpResponseMessage>(3, TimeSpan.FromMinutes(5));

        public async Task<IResult<IEnumerable<CryptoFearAndGreedData>, string>> GetCryptoFearAndGreedIndex(int numberOfDates)
        {
            using var httpResponseMessage = await _pollyPolicy.ExecuteAsync(() => _httpClient.GetAsync($"https://api.alternative.me/fng/?limit={numberOfDates}", HttpCompletionOption.ResponseHeadersRead));

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logger.LogError("{httpResponseMessage.StatusCode}", httpResponseMessage.StatusCode);
                return Result<IEnumerable<CryptoFearAndGreedData>, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
            }

            try
            {
                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<CryptoFearAndGreedRoot>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    logger.LogInformation("deserializedData '{@deserializedData}' ", deserializedData);
                    return Result<IEnumerable<CryptoFearAndGreedData>, string>.Success(deserializedData.CryptoFearAndGreedDatas);
                }

                logger.LogWarning("Deserialization Failed");
                return Result<IEnumerable<CryptoFearAndGreedData>, string>.Fail($"{nameof(GetCryptoFearAndGreedIndex)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("{exception}", exception);
                return Result<IEnumerable<CryptoFearAndGreedData>, string>.Fail(exception.ToString());
            }
        }
    }
}
