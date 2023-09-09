using HtmlAgilityPack;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public class WallStreetZenClient : IWallStreetZenClient
    {
        public IEnumerable<Stock> Sync()
        {
            var stockExchanges = new Dictionary<string, string>
            {
                { "xom", "nyse" },
                { "nke", "nyse" },
                { "ba", "nyse" },
                { "tsla", "nasdaq" },
                { "aapl", "nasdaq" },
                { "googl", "nasdaq" },
                { "abnb", "nasdaq" },
                { "brk.b", "nyse" },
            }.ToImmutableDictionary();

            List<Stock> stocks = new List<Stock>();

            foreach (var stockExchange in stockExchanges)
            {
                Stock stock = new Stock();
                stock.Symbol = stockExchange.Key;
                stock.Exchange = stockExchange.Value;
                //GetStockForecast(stock);
                //GetStockData(stock);
                stocks.Add(stock);
            };

            return stocks;
        }

        private static void GetStockForecast(Stock stock)
        {
            var urlStockForecast = $"https://www.wallstreetzen.com/stocks/us/{stock.Exchange}/{stock.Symbol}/stock-forecast";
            var web = new HtmlWeb();
            var document = web.Load(urlStockForecast);
            var innerText = document.DocumentNode.InnerText;

            const string minForecastPattern = @"Min Forecast\$([0-9.]+)([+-][0-9.]+)%";
            const string avgForecastPattern = @"Avg Forecast\$([0-9.]+)([+-][0-9.]+)%";
            const string maxForecastPattern = @"Max Forecast\$([0-9.]+)([+-][0-9.]+)%";

            Match minForecastMatch = Regex.Match(innerText, minForecastPattern);
            Match avgForecastMatch = Regex.Match(innerText, avgForecastPattern);
            Match maxForecastMatch = Regex.Match(innerText, maxForecastPattern);

            stock.MinForecast = decimal.Parse(minForecastMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            stock.MinForecastPercentage = decimal.Parse(minForecastMatch.Groups[2].Value, CultureInfo.InvariantCulture);
            stock.AvgForecast = decimal.Parse(avgForecastMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            stock.AvgForecastPercentage = decimal.Parse(avgForecastMatch.Groups[2].Value, CultureInfo.InvariantCulture);
            stock.MaxForecast = decimal.Parse(maxForecastMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            stock.MaxForecastPercentage = decimal.Parse(maxForecastMatch.Groups[2].Value, CultureInfo.InvariantCulture);
        }

        private static void GetStockData(Stock stock)
        {
            string urlStockSymbol = $"https://www.wallstreetzen.com/stocks/us/{stock.Exchange}/{stock.Symbol}";
            var web = new HtmlWeb();
            HtmlDocument document = web.Load(urlStockSymbol);

            const string fairValuePattern = @"Fair Value Price\$([0-9.]+)";
            const string undervaluedPattern = @"Undervalued by([0-9.]+)%";

            var innerText = document.DocumentNode.InnerText;

            Match fairValueMatch = Regex.Match(innerText, fairValuePattern);
            Match undervaluedMatch = Regex.Match(innerText, undervaluedPattern);

            if (fairValueMatch.Success)
            {
                stock.FairValuePrice = decimal.Parse(fairValueMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            if (undervaluedMatch.Success)
            {
                stock.UnderValuePercentage = decimal.Parse(undervaluedMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }
        }
    }
}
