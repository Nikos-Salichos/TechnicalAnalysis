using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Settings
{
    public class DexSetting
    {
        [Required]
        public string UniswapEndpoint { get; init; }

        [Required]
        public string PancakeswapEndpoint { get; init; }
    }
}
