﻿using Microsoft.Extensions.Logging;
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
    public class CoinRankingClient(IOptionsMonitor<CoinRankingSetting> settings, IHttpClientFactory httpClientFactory,
        ILogger<CoinRankingClient> logger, IPollyPolicy pollyPolicy) : ICoinRankingClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
        private readonly ResiliencePipeline _resiliencePipeline = pollyPolicy.CreatePolicies(retries: 3);

        public async Task<IResult<CoinRankingAssetContract, string>> SyncAssets(int offset)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("x-access-token", settings.CurrentValue.ApiKey);

                UriBuilder uriBuilder = new(settings.CurrentValue.ListingsLatestEndpoint ?? throw new ArgumentNullException("settings.CurrentValue.ListingsLatestEndpoint is null"));
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["tags"] = "layer-1";
                queryString["orderBy"] = "listedAt";
                queryString["limit"] = "100";

                uriBuilder.Query = queryString.ToString();

                using var httpResponseMessage = await _resiliencePipeline.ExecuteAsync(async (ctx)
                    => await _httpClient.GetAsync(uriBuilder.ToString(), HttpCompletionOption.ResponseHeadersRead, ctx));

                logger.LogInformation("SymbolsPairsPath {BaseUrl}, httpResponseMessage '{@HttpResponseMessage}' ",
                    settings.CurrentValue.ListingsLatestEndpoint, httpResponseMessage);

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogError("{HttpResponseMessageStatusCode}", httpResponseMessage.StatusCode);
                    return Result<CoinRankingAssetContract, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
                }

                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<CoinRankingAssetContract>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<CoinRankingAssetContract, string>.Success(deserializedData);
                }

                logger.LogError("Deserialization Failed");
                return Result<CoinRankingAssetContract, string>.Fail($"{nameof(SyncAssets)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("Method {Method}, Exception {Exception} ", nameof(SyncAssets), exception);
                return Result<CoinRankingAssetContract, string>.Fail(exception.Message);
            }
        }
    }
}
