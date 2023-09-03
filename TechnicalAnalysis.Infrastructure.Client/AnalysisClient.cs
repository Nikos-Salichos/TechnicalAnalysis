using System.Text.Json;
using TechnicalAnalysis.CommonModels;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Infrastructure.Client
{
    public class AnalysisClient : IAnalysisClient
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<IEnumerable<PartialPair>> GetPairsIndicators(Provider provider = Provider.All)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://your-api-base-url.com/");
            var response = await client.GetAsync("api/v1/analysis/SynchronizeProviders");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Providers Synchronize completed successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to Synchronize providers. Status Code: {response.StatusCode}");
            }

            using var jsonStream = await response.Content.ReadAsStreamAsync();
            var pairs = await JsonSerializer.DeserializeAsync<IEnumerable<PartialPair>>(jsonStream, _jsonSerializerOptions);
            pairs ??= Enumerable.Empty<PartialPair>();
            return pairs;
        }

        public Task Synchronize()
        {
            throw new NotImplementedException();
        }

        public Task GetIndicatorsByPairName(string pairName)
        {
            throw new NotImplementedException();
        }


    }
}
