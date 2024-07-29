using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Settings
{
    public class CoinPaprikaSetting
    {
        [Required]
        public string Endpoint { get; init; }
    }
}
