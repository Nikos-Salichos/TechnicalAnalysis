using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.Text.Json;
using TechnicalAnalysis.Domain.Contracts.Input.Binance;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class BinanceHttpClient(IOptionsMonitor<BinanceSetting> binanceSettings, IHttpClientFactory httpClientFactory,
        ILogger<BinanceHttpClient> logger, IPollyPolicy pollyPolicy) : IBinanceHttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("default");

        private readonly IAsyncPolicy<HttpResponseMessage> _pollyPolicy = pollyPolicy.CreatePolicies<HttpResponseMessage>(3, TimeSpan.FromMinutes(5));

        public async Task<IResult<BinanceExchangeInfoResponse, string>> GetBinanceAssetsAndPairs()
        {
            using var httpResponseMessage = await _pollyPolicy.ExecuteAsync(() => _httpClient.GetAsync(binanceSettings.CurrentValue.SymbolsPairsPath, HttpCompletionOption.ResponseHeadersRead));

            logger.LogInformation("SymbolsPairsPath {_binanceSettings.CurrentValue.SymbolsPairsPath}, httpResponseMessage '{@httpResponseMessage}' ",
             binanceSettings.CurrentValue.SymbolsPairsPath, httpResponseMessage);

            if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logger.LogError("{httpResponseMessage.StatusCode}", httpResponseMessage.StatusCode);
                return Result<BinanceExchangeInfoResponse, string>.Fail(httpResponseMessage.StatusCode + "" + httpResponseMessage.Content);
            }

            try
            {
                using var content = httpResponseMessage.Content;
                await using var jsonStream = await content.ReadAsStreamAsync();

                var deserializedData = await JsonSerializer.DeserializeAsync<BinanceExchangeInfoResponse>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    logger.LogInformation("deserializedData '{@deserializedData}' ", deserializedData);
                    return Result<BinanceExchangeInfoResponse, string>.Success(deserializedData);
                }

                logger.LogWarning("Deserialization Failed");
                return Result<BinanceExchangeInfoResponse, string>.Fail($"{nameof(GetBinanceAssetsAndPairs)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("{exception}", exception);
                return Result<BinanceExchangeInfoResponse, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<object[][], string>> GetBinanceCandlesticks(IDictionary<string, string>? queryParams = null)
        {
            var binanceCandlestickPath = binanceSettings.CurrentValue.CandlestickPath;
            if (queryParams != null)
            {
                binanceCandlestickPath += "?" + string.Join("&", queryParams.Select(p => $"{p.Key}={p.Value}"));
            }

            try
            {
                using var httpResponseMessage = await _pollyPolicy.ExecuteAsync(() => _httpClient.GetAsync(binanceCandlestickPath, HttpCompletionOption.ResponseHeadersRead));

                logger.LogInformation("binanceCandlestickPath {binanceCandlestickPath}, httpResponseMessage StatusCode {httpResponseMessage.StatusCode} ",
                    binanceCandlestickPath, httpResponseMessage.StatusCode.ToString());

                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogWarning("{httpResponseMessage.Content}", httpResponseMessage.Content);
                    return Result<object[][], string>.Fail(httpResponseMessage.StatusCode + " " + httpResponseMessage.Content);
                }

                await using var jsonStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var deserializedData = await JsonSerializer.DeserializeAsync<object[][]>(jsonStream, JsonHelper.JsonSerializerOptions);
                if (deserializedData is not null)
                {
                    return Result<object[][], string>.Success(deserializedData);
                }
                logger.LogWarning("Deserialization Failed");
                return Result<object[][], string>.Fail($"{nameof(GetBinanceCandlesticks)} Deserialization Failed");
            }
            catch (Exception exception)
            {
                logger.LogError("exception {exception}", exception);
                return Result<object[][], string>.Fail(exception.ToString());
            }
        }

        //TODO I do not need it
        /*        public void Dispose()
                {
                    _httpClient.Dispose();
                }*/
    }
}