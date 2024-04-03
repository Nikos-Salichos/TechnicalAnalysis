using Microsoft.Extensions.Logging;
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
    public class DexV3HttpClient(IOptionsMonitor<DexSetting> dexSettings, IHttpClientFactory httpClientFactory,
        ILogger<DexV3HttpClient> logger, IPollyPolicy pollyPolicy) : IDexV3HttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");

        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy = pollyPolicy.CreatePolicies<HttpResponseMessage>(3, TimeSpan.FromMinutes(5));

        public Task<IResult<DexV3ApiResponse, string>> GetMostActivePoolsAsync(int numberOfPools, int numberOfData, DataProvider provider)
        {
            const string query = @"
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
                      ";

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
                using var httpResponseMessage = await _retryPolicy.ExecuteAsync(() => _httpClient.PostAsJsonAsync(endpoint, body));

                logger.LogInformation("Method {Method}, dexEndpoint {dexEndpoint}, " +
                    "httpResponseMessage StatusCode {StatusCode}",
                        nameof(GetMostActivePoolsAsync), endpoint, httpResponseMessage.StatusCode);

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogWarning("{httpResponseMessage.StatusCode}", httpResponseMessage.StatusCode);
                    return Result<DexV3ApiResponse, string>.Fail(httpResponseMessage.StatusCode + " " + httpResponseMessage.Content);
                }

                await using var jsonStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var deserializedData = await JsonSerializer.DeserializeAsync<DexV3ApiResponse>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<DexV3ApiResponse, string>.Success(deserializedData);
                }
                logger.LogWarning("Method {Method}: Deserialization Failed", nameof(GetMostActivePoolsAsync));
                return Result<DexV3ApiResponse, string>.Fail($"{nameof(GetMostActivePoolsAsync)} Deserialization Failed");
            }
        }
    }
}
