﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Polly;
using System.Text.Json;
using System.Web;
using TechnicalAnalysis.Domain.Contracts.Input.CoinMarketCap;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class CoinMarketCapClient(IOptionsMonitor<CoinMarketCapSetting> coinMarketCapSettings, IHttpClientFactory httpClientFactory,
        ILogger<CoinMarketCapClient> logger, IPollyPolicy pollyPolicy) : ICoinMarketCapClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
        private readonly ResiliencePipeline _resiliencePipeline = pollyPolicy.CreatePolicies(retries: 3);

        public async Task<IResult<CoinMarketCapAssetContract, string>> SyncAssets()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", coinMarketCapSettings.CurrentValue.ApiKey);
                _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
                UriBuilder uriBuilder = new(coinMarketCapSettings.CurrentValue.ListingsLatestEndpoint
                    ?? throw new ArgumentOutOfRangeException("coinMarketCapSettings.CurrentValue.ListingsLatestEndpoint is null"));

                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["cryptocurrency_type"] = "coins";
                queryString["limit"] = "5000";
                queryString["convert"] = "USD";
                queryString["sort"] = "date_added";
                queryString["sort_dir"] = "desc";

                uriBuilder.Query = queryString.ToString();

                using var httpResponseMessage = await _resiliencePipeline.ExecuteAsync(async (ctx)
                    => await _httpClient.GetAsync(uriBuilder.ToString(), HttpCompletionOption.ResponseHeadersRead, ctx));

                logger.LogInformation("SymbolsPairsPath {BaseUrl}, httpResponseMessage '{@HttpResponseMessage}' ",
                    coinMarketCapSettings.CurrentValue.ListingsLatestEndpoint, httpResponseMessage);

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogError("{HttpResponseMessageStatusCode}", httpResponseMessage.StatusCode);
                    return Result<CoinMarketCapAssetContract, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
                }

                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<CoinMarketCapAssetContract>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<CoinMarketCapAssetContract, string>.Success(deserializedData);
                }

                logger.LogError("Deserialization Failed");
                return Result<CoinMarketCapAssetContract, string>.Fail($"{nameof(SyncAssets)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("Method {Method}, Exception {Exception} ", nameof(SyncAssets), exception);
                return Result<CoinMarketCapAssetContract, string>.Fail(exception.Message);
            }
        }
    }
}
