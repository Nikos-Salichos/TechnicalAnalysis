﻿using Microsoft.Extensions.Logging;
using Polly;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class CryptoFearAndGreedClient(IHttpClientFactory httpClientFactory, ILogger<CryptoFearAndGreedClient> logger, IPollyPolicy pollyPolicy)
        : ICryptoFearAndGreedClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
        private readonly ResiliencePipeline _resiliencePipeline = pollyPolicy.CreatePolicies(retries: 3);

        public async Task<IResult<List<CryptoFearAndGreedData>, string>> GetCryptoFearAndGreedIndex(int numberOfDates)
        {
            try
            {
                using var httpResponseMessage = await _resiliencePipeline.ExecuteAsync(async (ctx)
                    => await _httpClient.GetAsync($"https://api.alternative.me/fng/?limit={numberOfDates}", HttpCompletionOption.ResponseHeadersRead, ctx));

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogError("{HttpResponseMessageStatusCode}", httpResponseMessage.StatusCode);
                    return Result<List<CryptoFearAndGreedData>, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
                }

                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<CryptoFearAndGreedRoot>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<List<CryptoFearAndGreedData>, string>.Success(deserializedData.CryptoFearAndGreedDatas.ToList());
                }

                logger.LogError("Deserialization Failed");
                return Result<List<CryptoFearAndGreedData>, string>.Fail($"{nameof(GetCryptoFearAndGreedIndex)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("Method {Method}, Exception {Exception} ", nameof(GetCryptoFearAndGreedIndex), exception);
                return Result<List<CryptoFearAndGreedData>, string>.Fail(exception.Message);
            }
        }
    }
}
