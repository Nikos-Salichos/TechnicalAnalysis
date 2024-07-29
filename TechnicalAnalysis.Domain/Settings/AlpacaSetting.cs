using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Settings
{
    public class AlpacaSetting
    {
        [Required]
        public string ApiKey { get; init; }

        [Required]
        public string ApiSecret { get; init; }
    }
}
