﻿using HtmlAgilityPack;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Adapters.HttpClients
{
    public partial class WallStreetZenClient : IWallStreetZenClient
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
                // GetStockForecast(stock); //TODO In progress
                // GetStockData(stock); //TODO In progress
                stocks.Add(stock);
            }

            return stocks;
        }

        /*        public void GetFearAndGreed()
                {
                    const string urlStockForecast = "https://alternative.me/crypto/fear-and-greed-index/";
                    var web = new HtmlWeb();
                    var document = web.Load(urlStockForecast);
                    var innerText = document.DocumentNode.InnerText;
                    var todayStatusDiv = document.DocumentNode.SelectSingleNode("//div[@class='fng-value']//div[@class='status']");
                    var todayValueDiv = document.DocumentNode.SelectSingleNode("//div[@class='fng-value']//div[@class='fng-circle']");

                    if (todayStatusDiv != null && todayValueDiv != null)
                    {
                        string todayStatus = todayStatusDiv.InnerText.Trim();
                        string todayValue = todayValueDiv.InnerText.Trim();
                        Console.WriteLine("The extracted status is: " + todayStatus);
                        Console.WriteLine("The extracted value is: " + todayValue);
                    }
                    else
                    {
                        Console.WriteLine("Value not found");
                    }

                    //Example how to pick same div for example for yesterday
                    // var yesterdayValueNode = document.DocumentNode.SelectSingleNode("(//div[@class='fng-value'])[2]/div/div[@class='fng-circle']");
                }*/

        private static void GetStockForecast(Stock stock)
        {
            var urlStockForecast = $"https://www.wallstreetzen.com/stocks/us/{stock.Exchange}/{stock.Symbol}/stock-forecast";
            var web = new HtmlWeb();
            var document = web.Load(urlStockForecast);
            var innerText = document.DocumentNode.InnerText;

            Match minForecastMatch = MinimumPriceForecastPattern().Match(innerText);
            Match avgForecastMatch = AveragePriceForecastPattern().Match(innerText);
            Match maxForecastMatch = MaximumPriceForecastPattern().Match(innerText);

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

            var innerText = document.DocumentNode.InnerText;

            Match fairValueMatch = FairPricePattern().Match(innerText);
            Match undervaluedMatch = UndervaluePricePattern().Match(innerText);

            if (fairValueMatch.Success)
            {
                stock.FairValuePrice = decimal.Parse(fairValueMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            if (undervaluedMatch.Success)
            {
                stock.UnderValuePercentage = decimal.Parse(undervaluedMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            }
        }

        [GeneratedRegex("Min Forecast\\$([0-9.]+)([+-][0-9.]+)%")]
        private static partial Regex MinimumPriceForecastPattern();

        [GeneratedRegex("Avg Forecast\\$([0-9.]+)([+-][0-9.]+)%")]
        private static partial Regex AveragePriceForecastPattern();

        [GeneratedRegex("Max Forecast\\$([0-9.]+)([+-][0-9.]+)%")]
        private static partial Regex MaximumPriceForecastPattern();
        [GeneratedRegex("Fair Value Price\\$([0-9.]+)")]
        private static partial Regex FairPricePattern();
        [GeneratedRegex("Undervalued by([0-9.]+)%")]
        private static partial Regex UndervaluePricePattern();
    }
}
