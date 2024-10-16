﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.Net.Http.Json;
using System.Text.Json;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class DexV3Client(IOptionsMonitor<DexSetting> dexSettings, IHttpClientFactory httpClientFactory,
        ILogger<DexV3Client> logger, IPollyPolicy pollyPolicy) : IDexV3Client
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");
        private readonly ResiliencePipeline _resiliencePipeline = pollyPolicy.CreatePolicies(retries: 3);

        public Task<IResult<DexV3ApiResponse, string>> GetMostActivePoolsAsync(int numberOfPools, int numberOfData, DataProvider provider)
        {
            const string query = """
                                 
                                                           query($numberOfPools: Int!, $numberOfData: Int! ) {
                                                               pools(first: $numberOfPools, orderBy: txCount, orderDirection: desc) {
                                                                   id
                                                                   feeTier
                                                                   liquidity
                                                                   totalValueLockedUSD
                                                                   volumeUSD
                                                                   feesUSD
                                                                   txCount
                                                                   poolDayData(first:  $numberOfData, skip: 1, orderBy: date, orderDirection: desc) {
                                                                       id
                                                                       date
                                                                       feesUSD
                                                                       liquidity
                                                                       tvlUSD
                                                                       volumeUSD
                                                                       txCount
                                                                   }
                                                                   token0 {
                                                                       id
                                                                       symbol
                                                                       name
                                                                       tokenDayData(first:  $numberOfData, skip: 1, orderBy: date, orderDirection: desc) {
                                                                           open
                                                                           high
                                                                           low
                                                                           close
                                                                           date
                                                                       }
                                                                   }
                                                                   token1 {
                                                                       id
                                                                       symbol
                                                                       name
                                                                       tokenDayData(first:  $numberOfData, skip: 1, orderBy: date, orderDirection: desc) {
                                                                           open
                                                                           high
                                                                           low
                                                                           close
                                                                           date
                                                                       }
                                                                   }
                                                               }
                                                           }
                                                       
                                 """;

            var requestBody = new
            {
                query,
                variables = new
                {
                    numberOfPools,
                    numberOfData
                }
            };

            var dexEndpoint = provider switch
            {
                DataProvider.Uniswap => dexSettings.CurrentValue.UniswapEndpoint,
                DataProvider.Pancakeswap => dexSettings.CurrentValue.PancakeswapEndpoint,
                _ => throw new InvalidOperationException("Unknown provider")
            };

            return FetchDataAsync(dexEndpoint, requestBody);

            async Task<IResult<DexV3ApiResponse, string>> FetchDataAsync(string endpoint, object body)
            {
                try
                {
                    using var httpResponseMessage = await _resiliencePipeline.ExecuteAsync(async (ctx)
                        => await _httpClient.PostAsJsonAsync(endpoint, body, ctx));

                    logger.LogInformation("Method {Method}, dexEndpoint {DexEndpoint}, " +
                        "httpResponseMessage StatusCode {StatusCode}",
                            nameof(GetMostActivePoolsAsync), endpoint, httpResponseMessage.StatusCode);

                    if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        logger.LogError("{HttpResponseMessageStatusCode}", httpResponseMessage.StatusCode);
                        return Result<DexV3ApiResponse, string>.Fail(httpResponseMessage.StatusCode + " " + httpResponseMessage.Content);
                    }

                    await using var jsonStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    var deserializedData = await JsonSerializer.DeserializeAsync<DexV3ApiResponse>(jsonStream, JsonHelper.JsonSerializerOptions);
                    if (deserializedData is not null)
                    {
                        return Result<DexV3ApiResponse, string>.Success(deserializedData);
                    }
                    logger.LogError("Method {Method}: Deserialization Failed", nameof(GetMostActivePoolsAsync));
                    return Result<DexV3ApiResponse, string>.Fail($"{nameof(GetMostActivePoolsAsync)} Deserialization Failed");
                }
                catch (Exception exception)
                {
                    logger.LogError("Method {Method}, Exception {Exception} ", nameof(FetchDataAsync), exception);
                    return Result<DexV3ApiResponse, string>.Fail(exception.Message);
                }
            }
        }
    }
}
