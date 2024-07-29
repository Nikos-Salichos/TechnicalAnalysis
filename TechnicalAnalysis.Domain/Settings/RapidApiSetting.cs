using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Settings
{
    public class RapidApiSetting
    {
        [Required]
        public string StockFearAndGreedUri { get; init; }

        [Required]
        public string StockFearAndGreedHost { get; init; }

        [Required]
        public string StockFearAndGreedApiKey { get; init; }
    }
}
