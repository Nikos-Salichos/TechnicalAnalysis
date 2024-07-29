using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Settings
{
    public class CoinRankingSetting
    {
        [Required]
        public string ApiKey { get; init; }

        [Required]
        public string ListingsLatestEndpoint { get; init; }
    }
}
