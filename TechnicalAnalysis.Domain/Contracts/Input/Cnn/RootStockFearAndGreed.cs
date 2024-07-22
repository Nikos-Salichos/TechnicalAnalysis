using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Domain.Contracts.Input.Cnn
{
    public record RootStockFearAndGreed(
        [property: JsonPropertyName("fear_and_greed_historical")] CnnFearAndGreedHistorical FearAndGreedHistorical
    //Pending to add more properties as needed
    /*,
    [property: JsonPropertyName("market_momentum_sp500")] MarketMomentumSp500 MarketMomentumSp500,
    [property: JsonPropertyName("market_momentum_sp125")] MarketMomentumSp125 MarketMomentumSp125,
    [property: JsonPropertyName("stock_price_strength")] StockPriceStrength StockPriceStrength,
    [property: JsonPropertyName("stock_price_breadth")] StockPriceBreadth StockPriceBreadth,
    [property: JsonPropertyName("put_call_options")] PutCallOptions PutCallOptions,
    [property: JsonPropertyName("market_volatility_vix")] MarketVolatilityVix MarketVolatilityVix,
    [property: JsonPropertyName("market_volatility_vix_50")] MarketVolatilityVix50 MarketVolatilityVix50,
    [property: JsonPropertyName("junk_bond_demand")] JunkBondDemand JunkBondDemand,
    [property: JsonPropertyName("safe_haven_demand")] SafeHavenDemand SafeHavenDemand
    */
    );

    public record CnnFearAndGreedHistoricalData(
        [property: JsonPropertyName("x")] double Timestamp,
        [property: JsonPropertyName("y")] double Score,
        [property: JsonPropertyName("rating")] string? Rating
    );

    public record CnnFearAndGreedHistorical(
        [property: JsonPropertyName("data")] List<CnnFearAndGreedHistoricalData> FearAndGreedHistoricalData
    );
}
