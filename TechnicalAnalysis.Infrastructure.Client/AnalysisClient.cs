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

        private readonly IHttpClientFactory _httpClientFactory;

        public AnalysisClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SynchronizeAsync(DataProvider provider = DataProvider.All)
        {
            var httpClient = _httpClientFactory.CreateClient("AnalysisClient");
            var apiUrl = $"SynchronizeProviders?provider={provider}";
            var response = await httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to synchronize providers. Status code: {response.StatusCode}, Content: {content}");
            }
        }

        public async Task<IEnumerable<PartialPair>> GetPairsIndicatorsAsync(DataProvider provider = DataProvider.All)
        {
            var httpClient = _httpClientFactory.CreateClient("AnalysisClient");
            var apiUrl = $"PairsIndicators?provider={provider}";
            var response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                using var jsonStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<IEnumerable<PartialPair>>(jsonStream, _jsonSerializerOptions)
                    ?? Enumerable.Empty<PartialPair>();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to get pairs indicators. Status code: {response.StatusCode}, Content: {content}");
            }
        }

        public async Task<IEnumerable<PairExtended>> GetIndicatorsByPairName(string pairName)
        {
            var httpClient = _httpClientFactory.CreateClient("AnalysisClient");
            var apiUrl = $"IndicatorsByPairName?pairName={pairName}";
            var response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                using var jsonStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<IEnumerable<PairExtended>>(jsonStream, _jsonSerializerOptions)
                    ?? Enumerable.Empty<PairExtended>();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to calculate pair's indicators. Status code: {response.StatusCode}, Content: {content}");
            }
        }

    }
}
