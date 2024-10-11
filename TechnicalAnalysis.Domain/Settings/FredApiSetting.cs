using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Settings
{
    public class FredApiSetting
    {
        [Required]
        public string ApiKey { get; init; }

        [Required]
        public string VixEndpoint { get; init; }
    }
}
