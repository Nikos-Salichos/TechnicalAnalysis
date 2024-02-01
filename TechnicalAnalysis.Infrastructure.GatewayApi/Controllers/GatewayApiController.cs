using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Infrastructure.GatewayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiGatewayController(IHttpClientFactory httpClientFactory) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("gatewayapi");

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [HttpGet]
        public async Task<IActionResult> GetIndicatorsByPairNameAsync([FromQuery] string pairName, [FromQuery] Timeframe timeframe)
        {
            const string apiUrl = "IndicatorsByPairName";
            var requestData = new { PairName = pairName, Timeframe = timeframe };
            return Ok(await SendHttpRequestAsync<IEnumerable<PairExtended>>(apiUrl, HttpMethod.Get, requestData));
        }

        private async Task<T?> SendHttpRequestAsync<T>(string apiUrl, HttpMethod httpMethod, object? requestData = null)
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

            if (response.Content is null || response.Content.Headers.ContentLength == 0)
            {
                return default;
            }

            await using var jsonStream = await response.Content.ReadAsStreamAsync();
            var deserializedResponse = await JsonSerializer.DeserializeAsync<T>(jsonStream, _jsonSerializerOptions);
            return deserializedResponse ?? default;
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
                if (value is not null)
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
