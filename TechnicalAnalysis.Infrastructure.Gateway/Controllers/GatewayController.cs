using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Contracts.Output;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TechnicalAnalysis.Infrastructure.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController(IHttpClientFactory httpClientFactory) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("taapi");

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [HttpGet("IndicatorsByPairName")]
        public async Task<IActionResult> GetIndicatorsByPairNameAsync([FromQuery] List<string> pairNames, [FromQuery] Timeframe timeframe)
        {
            const string apiUrl = "IndicatorsByPairName";
            var requestData = new { pairNames, timeframe };

            var result = await SendHttpRequestAsync<List<PairExtended>>(apiUrl, HttpMethod.Get, requestData);
            return Ok(result);
        }

        [HttpGet("EnhancedIndicatorPairResults")]
        public async Task<IActionResult> GetEnhancedIndicatorPairResultsAsync([FromQuery] DataProvider dataProvider)
        {
            const string apiUrl = "EnhancedIndicatorPairResults";
            var requestData = new { dataProvider };

            var result = await SendHttpRequestAsync<List<EnhancedPairResult>>(apiUrl, HttpMethod.Get, requestData);
            return Ok(result);
        }

        private async Task<T?> SendHttpRequestAsync<T>(string apiUrl, HttpMethod httpMethod, object? requestData = null, Dictionary<string, string>? headers = null)
        {
            HttpResponseMessage response;

            if (httpMethod == HttpMethod.Get)
            {
                var urlWithParams = apiUrl + (requestData != null ? ToQueryString(requestData) : "");

                HttpRequestMessage request = new()
                {
                    RequestUri = new Uri(_httpClient.BaseAddress + urlWithParams),
                    Method = httpMethod
                };

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                response = await _httpClient.SendAsync(request);
            }
            else if (httpMethod == HttpMethod.Post)
            {
                var urlWithParams = apiUrl + (requestData != null ? ToQueryString(requestData) : "");
                HttpRequestMessage request = new()
                {
                    RequestUri = new Uri(_httpClient.BaseAddress + urlWithParams),
                    Method = httpMethod,
                    Content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json")
                };
                response = await _httpClient.SendAsync(request);
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

            if (response.Content is null || response.Content.Headers.ContentLength == 0)
            {
                return default;
            }

            await using var jsonStream = await response.Content.ReadAsStreamAsync();
            var deserializedResponse = await JsonSerializer.DeserializeAsync<T>(jsonStream, _jsonSerializerOptions);

            return deserializedResponse ?? default;
        }

        private static string ToQueryString(object? requestData)
        {
            if (requestData is null)
            {
                return string.Empty;
            }

            var queryStringComponents = new List<string>();

            foreach (var property in requestData.GetType().GetProperties())
            {
                var value = property.GetValue(requestData);
                if (value is not null)
                {
                    string escapedName = Uri.EscapeDataString(property.Name);
                    string escapedValue = Uri.EscapeDataString(value.ToString() ?? string.Empty);
                    queryStringComponents.Add($"{escapedName}={escapedValue}");
                }
            }

            return queryStringComponents.Count > 0
                ? "?" + string.Join("&", queryStringComponents)
                : string.Empty;
        }
    }
}
