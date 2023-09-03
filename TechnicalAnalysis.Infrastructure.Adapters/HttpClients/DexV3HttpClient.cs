using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly.Timeout;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Input.DexV3;
using TechnicalAnalysis.Domain.Interfaces;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Settings;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class DexV3HttpClient : IDexV3HttpClient
    {
        private readonly IOptionsMonitor<DexSetting> _dexSettings;
        private readonly ILogger<DexV3HttpClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncTimeoutPolicy _asyncTimeoutPolicy;

        public DexV3HttpClient(IOptionsMonitor<DexSetting> dexSettings, IHttpClientFactory httpClientFactory,
            ILogger<DexV3HttpClient> logger, IPollyPolicy pollyPolicy)
        {
            _dexSettings = dexSettings;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _retryPolicy = pollyPolicy.CreateRetryPolicy(3, TimeSpan.FromSeconds(5));
            _asyncTimeoutPolicy = pollyPolicy.CreateTimeoutPolicy(TimeSpan.FromMinutes(5));
        }

        public async Task<DexV3ApiResponse> GetMostActivePools(int numberOfPools, int numberOfData, Provider provider)
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
                Provider.Uniswap => _dexSettings.CurrentValue.UniswapEndpoint,
                Provider.Pancakeswap => _dexSettings.CurrentValue.PancakeswapEndpoint,
                _ => throw new InvalidOperationException("Unknown provider")
            };

            var httpclient = _httpClientFactory.CreateClient();
            httpclient.DefaultRequestHeaders.Add("User-Agent", "Chrome application");

            using var httpResponseMessage = await _retryPolicy.WrapAsync(_asyncTimeoutPolicy)
                                                              .ExecuteAsync(() => httpclient.PostAsJsonAsync(dexEndpoint, requestBody));

            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using var jsonStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<DexV3ApiResponse>(jsonStream, _jsonSerializerOptions)
                ?? new DexV3ApiResponse();
            }
            else
            {
                _logger.LogError("{Method}: {httpResponseMessage.StatusCode}", MethodBase.GetCurrentMethod()?.Name, httpResponseMessage.StatusCode);
                return new DexV3ApiResponse();
            }
        }
    }
}
