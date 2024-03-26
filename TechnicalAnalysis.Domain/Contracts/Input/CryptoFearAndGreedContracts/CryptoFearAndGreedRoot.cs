using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.CryptoFearAndGreedContracts
{
    public class CryptoFearAndGreedRoot
    {
        [JsonPropertyName("data")]
        public IEnumerable<CryptoFearAndGreedData> CryptoFearAndGreedDatas { get; set; } = new List<CryptoFearAndGreedData>();
    }
}
