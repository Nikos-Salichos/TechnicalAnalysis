﻿using Microsoft.Extensions.Logging;
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
    public class StockFearAndGreedHttpClient(IOptionsMonitor<RapidApiSetting> rapidApiSetting, IHttpClientFactory httpClientFactory,
        ILogger<StockFearAndGreedHttpClient> logger, IPollyPolicy pollyPolicy) : IStockFearAndGreedHttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");

        private readonly IAsyncPolicy<HttpResponseMessage> _pollyPolicy = pollyPolicy.CreatePolicies<HttpResponseMessage>(3, TimeSpan.FromMinutes(5));

        public async Task<IResult<StockFearAndGreedRoot, string>> GetStockFearAndGreedIndex()
        {
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", rapidApiSetting.CurrentValue.StockFearAndGreedApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", rapidApiSetting.CurrentValue.StockFearAndGreedHost);
            using var httpResponseMessage = await _pollyPolicy.ExecuteAsync(() => _httpClient.GetAsync(rapidApiSetting.CurrentValue.StockFearAndGreedUri, HttpCompletionOption.ResponseHeadersRead));

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logger.LogError("{httpResponseMessage.StatusCode}", httpResponseMessage.StatusCode);
                return Result<StockFearAndGreedRoot, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
            }

            using var content = httpResponseMessage.Content;
            await using var jsonStream = await content.ReadAsStreamAsync();

            var deserializedData = await JsonSerializer.DeserializeAsync<StockFearAndGreedRoot>(jsonStream, JsonHelper.JsonSerializerOptions);
            if (deserializedData is not null)
            {
                logger.LogInformation("deserializedData '{@deserializedData}' ", deserializedData);
                return Result<StockFearAndGreedRoot, string>.Success(deserializedData);
            }

            logger.LogWarning("Deserialization Failed");
            return Result<StockFearAndGreedRoot, string>.Fail($"{nameof(GetStockFearAndGreedIndex)} Deserialization Failed");
        }
    }
}
