using System.Text.Json.Serialization;

namespace TechnicalAnalysis.CommonModels.JsonOutput
{
    public class Indicator
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }
        public string PairName { get; init; }

        [JsonPropertyName("signals")]
        public IList<Signal> Signals { get; init; } = new List<Signal>();
    }
}
