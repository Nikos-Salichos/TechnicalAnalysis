using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Settings
{
    public class CnnApiSetting
    {
        [Required]
        public string StockFearAndGreedUri { get; init; }
    }
}
