using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly.Timeout;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;
using TechnicalAnalysis.Domain.Interfaces;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class BinanceHttpClient : IBinanceHttpClient
    {
        private readonly IOptionsMonitor<BinanceSetting> _binanceSettings;
        private readonly ILogger<BinanceHttpClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncTimeoutPolicy _asyncTimeoutPolicy;

        public BinanceHttpClient(IOptionsMonitor<BinanceSetting> binanceSettings, IHttpClientFactory httpClientFactory,
            ILogger<BinanceHttpClient> logger, IPollyPolicy pollyPolicy)
        {
            _logger = logger;
            _binanceSettings = binanceSettings;
            _httpClientFactory = httpClientFactory;
            _retryPolicy = pollyPolicy.CreateRetryPolicy(3, TimeSpan.FromSeconds(5));
            _asyncTimeoutPolicy = pollyPolicy.CreateTimeoutPolicy(TimeSpan.FromMinutes(5));
        }

        public async Task<IResult<BinanceExchangeInfoResponse, string>> GetBinanceAssetsAndPairs()
        {
            var httpclient = _httpClientFactory.CreateClient();
            httpclient.DefaultRequestHeaders.Add("User-Agent", "Chrome application");

            using var httpResponseMessage = await _retryPolicy.WrapAsync(_asyncTimeoutPolicy)
                                                              .ExecuteAsync(() => httpclient.GetAsync(_binanceSettings.CurrentValue.SymbolsPairsPath, HttpCompletionOption.ResponseHeadersRead));

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError("Method: {Method} {httpResponseMessage.StatusCode}", nameof(GetBinanceAssetsAndPairs), httpResponseMessage.StatusCode);
                return Result<BinanceExchangeInfoResponse, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
            }

            try
            {
                using var jsonStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var deserializedData = await JsonSerializer.DeserializeAsync<BinanceExchangeInfoResponse>(jsonStream, _jsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<BinanceExchangeInfoResponse, string>.Success(deserializedData);
                }

                _logger.LogWarning("Method: {Method} Deserialization Failed", nameof(GetBinanceAssetsAndPairs));
                return Result<BinanceExchangeInfoResponse, string>.Fail($"{nameof(GetBinanceAssetsAndPairs)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                _logger.LogError("Method: {Method} {exception}", nameof(GetBinanceAssetsAndPairs), exception);
                return Result<BinanceExchangeInfoResponse, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<object[][], string>> GetBinanceCandlesticks(IDictionary<string, string>? queryParams = null)
        {
            var httpclient = _httpClientFactory.CreateClient();
            httpclient.DefaultRequestHeaders.Add("User-Agent", "Chrome application");

            var binanceCandlestickPath = _binanceSettings.CurrentValue.CandlestickPath;
            if (queryParams != null)
            {
                binanceCandlestickPath += "?" + string.Join("&", queryParams.Select(p => $"{p.Key}={p.Value}"));
            }

            var headers = new Dictionary<string, string>
                    {
                        { "X-MBX-APIKEY" , _binanceSettings.CurrentValue.ApiKey },
                    };

            using var httpResponseMessage = await _retryPolicy.WrapAsync(_asyncTimeoutPolicy)
                                                              .ExecuteAsync(() => httpclient.GetAsync(binanceCandlestickPath, HttpCompletionOption.ResponseHeadersRead));

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogWarning("Method: {Method}: {httpResponseMessage.StatusCode}", nameof(GetBinanceCandlesticks), httpResponseMessage.StatusCode);
                return Result<object[][], string>.Fail(httpResponseMessage.StatusCode + " " + httpResponseMessage.Content);
            }

            try
            {
                using var jsonStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                object[][]? deserializedData = await JsonSerializer.DeserializeAsync<object[][]>(jsonStream, _jsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<object[][], string>.Success(deserializedData);
                }
                _logger.LogWarning("Method: {Method} {Deserialization Failed}", nameof(GetBinanceCandlesticks));
                return Result<object[][], string>.Fail($"{nameof(GetBinanceCandlesticks)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Method: {Method} {exception}", nameof(GetBinanceCandlesticks), exception);
                return Result<object[][], string>.Fail(exception.ToString());
            }
        }
    }
}
