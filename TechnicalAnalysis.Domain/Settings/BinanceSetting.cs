using System.ComponentModel.DataAnnotations;

namespace TechnicalAnalysis.Domain.Settings
{
    public class BinanceSetting
    {
        [Required]
        public string ApiKey { get; init; }

        [Required]
        public string SymbolsPairsPath { get; init; }

        [Required]
        public string CandlestickPath { get; init; }
    }
}
