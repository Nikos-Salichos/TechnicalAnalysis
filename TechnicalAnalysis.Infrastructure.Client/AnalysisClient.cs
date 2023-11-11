using System.Text;
using System.Text.Json;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.CommonModels.JsonOutput;

namespace TechnicalAnalysis.Infrastructure.Client
{
    public class AnalysisClient : IAnalysisClient
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public AnalysisClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AnalysisClient");
        }

        public async Task SynchronizeProvidersAsync(DataProvider provider, Timeframe timeframe)
        {
            var apiUrl = $"SynchronizeProviders?dataProvider={provider}&timeframe={timeframe}";
            await SendHttpRequestAsync<object>(apiUrl, HttpMethod.Get);
        }

        public Task<IEnumerable<PartialPair>> GetPairsIndicatorsAsync(DataProvider provider)
        {
            var apiUrl = $"PairsIndicators?provider={provider}";
            return SendHttpRequestAsync<IEnumerable<PartialPair>>(apiUrl, HttpMethod.Get);
        }

        public Task<IEnumerable<PairExtended>> GetIndicatorsByPairNameAsync(string pairName, Timeframe timeframe)
        {
            const string apiUrl = "IndicatorsByPairName";
            var requestData = new { PairName = pairName, Timeframe = timeframe };
            return SendHttpRequestAsync<IEnumerable<PairExtended>>(apiUrl, HttpMethod.Get, requestData);
        }

        private async Task<T> SendHttpRequestAsync<T>(string apiUrl, HttpMethod httpMethod, object? requestData = null)
        {
            try
            {
                HttpResponseMessage response;

                if (httpMethod == HttpMethod.Get)
                {
                    var urlWithParams = apiUrl + (requestData != null ? ToQueryString(requestData) : "");
                    response = await _httpClient.GetAsync(urlWithParams);
                }
                else if (httpMethod == HttpMethod.Post)
                {
                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync(apiUrl, jsonContent);
                }
                else
                {
                    throw new ArgumentException($"Unsupported HTTP method: {httpMethod}");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed to execute HTTP request. Status code: {response.StatusCode}, Content: {content}");
                }

                using var jsonStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(jsonStream, _jsonSerializerOptions)
                    ?? default;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Failed to execute HTTP request. {ex.Message}", ex);
            }
        }

        private static string ToQueryString(object requestData)
        {
            if (requestData is null)
            {
                return string.Empty;
            }

            var queryStringComponents = new List<string>();

            foreach (var property in requestData.GetType().GetProperties())
            {
                var value = property.GetValue(requestData);
                if (value != null)
                {
                    string escapedName = Uri.EscapeDataString(property.Name);
                    string escapedValue = Uri.EscapeDataString(value.ToString());
                    queryStringComponents.Add($"{escapedName}={escapedValue}");
                }
            }

            return queryStringComponents.Count > 0
                ? "?" + string.Join("&", queryStringComponents)
                : string.Empty;
        }
    }
}
