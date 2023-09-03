namespace TechnicalAnalysis.CommonModels.Enums
{
    public static class Constants
    {
        public const string Usd = "Usd";
        public const string Usdc = "USDC";
        public const string Usdt = "USDT";
        public const string Dai = "DAI";
        public const string Busd = "BUSD";

        public const double RsiOverbought = 70;
        public const double RsiOversold = 30;
        public const double StochasticOverbought = 80;
        public const double StochasticOversold = 20;
        public const double CciOversold = -100;
        public const double AdxOversold = 15;
        public const double AdxTrendLevel = 0;
        public const decimal BodyOneToTenOfRangeRatio = 0.1m;
        public const double MacdOversold = 0;
        public const double RateOfChangeOversold = -10;
        public const int RateOfChangePeriod = 7;

        public const int decimalPlacesRound = 20;
    }
}
