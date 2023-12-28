namespace TechnicalAnalysis.Domain.Settings
{
    public class BinanceSetting
    {
        public string ApiKey { get; init; } = string.Empty;
        public string SymbolsPairsPath { get; init; } = string.Empty;
        public string CandlestickPath { get; set; } = string.Empty;
    }
}
