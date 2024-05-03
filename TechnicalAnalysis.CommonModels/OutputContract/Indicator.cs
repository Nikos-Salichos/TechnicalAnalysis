using System.Text.Json.Serialization;

namespace TechnicalAnalysis.CommonModels.OutputContract
{
    public class Indicator
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }
        public string PairName { get; init; }

        [JsonPropertyName("signals")]
        public List<Signal> Signals { get; init; } = [];
    }
}
