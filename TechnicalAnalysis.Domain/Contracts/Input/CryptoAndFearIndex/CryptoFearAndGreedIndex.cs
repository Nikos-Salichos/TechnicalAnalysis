using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CryptoAndFearIndex
{
    public class CryptoFearAndGreedIndex
    {
        [JsonPropertyName("data")]
        public IEnumerable<CryptoFearAndGreedData> CryptoFearAndGreedDatas { get; set; } = new List<CryptoFearAndGreedData>();
    }
}
