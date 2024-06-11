using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.Cnn
{
    public class RootStockFearAndGreed
    {
        [JsonPropertyName("fear_and_greed_historical")]
        public CnnFearAndGreedHistorical FearAndGreedHistorical { get; init; }

        //Pending to add more
        /* [JsonPropertyName("market_momentum_sp500")]
         public MarketMomentumSp500 MarketMomentumSp500 { get; set; }

         [JsonPropertyName("market_momentum_sp125")]
         public MarketMomentumSp125 MarketMomentumSp125 { get; set; }

         [JsonPropertyName("stock_price_strength")]
         public StockPriceStrength StockPriceStrength { get; set; }

         [JsonPropertyName("stock_price_breadth")]
         public StockPriceBreadth StockPriceBreadth { get; set; }

         [JsonPropertyName("put_call_options")]
         public PutCallOptions PutCallOptions { get; set; }

         [JsonPropertyName("market_volatility_vix")]
         public MarketVolatilityVix MarketVolatilityVix { get; set; }

         [JsonPropertyName("market_volatility_vix_50")]
         public MarketVolatilityVix50 MarketVolatilityVix50 { get; set; }

         [JsonPropertyName("junk_bond_demand")]
         public JunkBondDemand JunkBondDemand { get; set; }

         [JsonPropertyName("safe_haven_demand")]
         public SafeHavenDemand SafeHavenDemand { get; set; }*/
    }

    public class CnnFearAndGreedHistoricalData
    {
        [JsonPropertyName("x")]
        public double Timestamp { get; init; }

        [JsonPropertyName("y")]
        public double Score { get; init; }

        [JsonPropertyName("rating")]
        public string? Rating { get; init; }
    }

    public class CnnFearAndGreedHistorical
    {
        [JsonPropertyName("data")]
        public List<CnnFearAndGreedHistoricalData> FearAndGreedHistoricalData { get; init; } = [];
    }
}
