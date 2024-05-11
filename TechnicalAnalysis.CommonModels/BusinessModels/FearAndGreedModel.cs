using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class FearAndGreedModel
    {
        public string Value { get; init; } = string.Empty;

        public ValueClassificationType ValueClassificationType { get; init; }

        public DateTime DateTime { get; init; } = DateTime.UtcNow;
    }
}
