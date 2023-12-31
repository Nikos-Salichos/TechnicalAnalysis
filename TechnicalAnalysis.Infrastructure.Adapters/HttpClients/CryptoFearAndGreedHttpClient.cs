using Microsoft.Extensions.Logging;
using Polly;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoAndFearIndex;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class CryptoFearAndGreedHttpClient(IHttpClientFactory httpClientFactory, ILogger<BinanceHttpClient> logger, IPollyPolicy pollyPolicy)
        : ICryptoFearAndGreedHttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");

        private readonly IAsyncPolicy<HttpResponseMessage> _pollyPolicy = pollyPolicy.CreatePolicies<HttpResponseMessage>(3, TimeSpan.FromMinutes(5));

        public async Task<IResult<IEnumerable<CryptoFearAndGreedData>, string>> GetCryptoFearAndGreedIndex(int numberOfDates)
        {
            using var httpResponseMessage = await _pollyPolicy.ExecuteAsync(() => _httpClient.GetAsync($"https://api.alternative.me/fng/?limit={numberOfDates}", HttpCompletionOption.ResponseHeadersRead));

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logger.LogError("Method: {Method} {httpResponseMessage.StatusCode}", nameof(GetCryptoFearAndGreedIndex), httpResponseMessage.StatusCode);
                return Result<IEnumerable<CryptoFearAndGreedData>, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
            }

            try
            {
                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<CryptoFearAndGreedIndex>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    logger.LogInformation("Method: {Method}, deserializedData '{@deserializedData}' ", nameof(GetCryptoFearAndGreedIndex), deserializedData);
                    return Result<IEnumerable<CryptoFearAndGreedData>, string>.Success(deserializedData.CryptoFearAndGreedDatas);
                }

                logger.LogWarning("Method: {Method} Deserialization Failed", nameof(GetCryptoFearAndGreedIndex));
                return Result<IEnumerable<CryptoFearAndGreedData>, string>.Fail($"{nameof(GetCryptoFearAndGreedIndex)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("Method: {Method} {exception}", nameof(GetCryptoFearAndGreedIndex), exception);
                return Result<IEnumerable<CryptoFearAndGreedData>, string>.Fail(exception.ToString());
            }
        }
    }
}
