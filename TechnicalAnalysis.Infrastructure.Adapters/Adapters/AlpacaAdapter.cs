using Alpaca.Markets;
using MediatR;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.Application.Extensions;
using TechnicalAnalysis.Application.Mediatr.Commands.Insert;
using TechnicalAnalysis.Application.Mediatr.Commands.Update;
using TechnicalAnalysis.Application.Mediatr.Queries;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Builders;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using Asset = TechnicalAnalysis.CommonModels.BusinessModels.Asset;
using DataProvider = TechnicalAnalysis.CommonModels.Enums.DataProvider;
using PairExtended = TechnicalAnalysis.CommonModels.BusinessModels.PairExtended;

namespace TechnicalAnalysis.Infrastructure.Adapters.Adapters
{
    public class AlpacaAdapter(ILogger<AlpacaAdapter> logger, IMediator mediator, IAlpacaHttpClient alpacaHttpClient) : IAdapter
    {
        static readonly List<string> etfTickers =
            [
               "vt", "vti", "VTV", "PFF", "SPHD", "XLRE", "EWH", "MCHI", "EWS", "FEZ",
               "IWM", "SPY", "DIA", "DAX", "VGK", "QQQ", "IVV", "VUG", "VB", "VNQ",
               "XLE", "XLF", "BND", "vea", "VWO", "GLD", "VXUS", "VO", "JEPI", "SPYV",
               "VOT", "VDE", "voo", "ITOT", "MEDP", "ELF", "URTH"
            ];

        public async Task<bool> Sync(DataProvider provider, Timeframe timeframe, List<ProviderSynchronization> exchanges)
        {
            var alpacaProvider = exchanges.Find(p => p.ProviderPairAssetSyncInfo.DataProvider == provider);

            alpacaProvider ??= new ProviderSynchronization
            {
                ProviderPairAssetSyncInfo = new ProviderPairAssetSyncInfo { DataProvider = provider }
            };

            List<string> tenStocksToOwnForever =
                [
                "ZTS","ODFL", "SYK", "LVMH", "CSU", "MKL", "WSO", "BRO", "BRK"
                ];

            //Add more stocks from here: https://www.wallstreetzen.com/stock-screener?p=1&s=mc&sd=desc&t=1
            //Check top owned stocks: https://www.dataroma.com/m/home.php
            List<string> stockTickers =
                [
                    "nke", "ba", "tsla", "aapl", "abnb", "WMT",
                    "WCF", "ABDE", "RY", "CRM", "VZ",
                    "IWD", "IJH", "BRK.A", "GIS", "KLG", "KHC", "MDLZ",
                    "NVO", "TM", "MRK", "GOOG", "DVA",
                    "KR", "WSO", "DE", "AME", "MKL", "HEI", "NVR", "TDG", "BN",
                    "RACE", "MU", "DJCO", "SYK", "PM", "ADP", "WAT", "PEP", "MKC", "FIS",
                    "WTW", "CRH", "WBA", "WSO", "ASML", "ADBE", "REGN", "FTNT", "ODFL", "CPRT", "ACN",
                    "LOW", "MO", "SJW","TGT","PPG", "SPGI", "BLK", "TMO", "UNP", "DHR", "IDXX", "RMV",
                    "EL", "PAYC","ORLY","ROP", "ANET", "LLY",
                ];

            List<string> dividendAristocrats =
                [
                    "O",         // Realty Income Corp.
                    "STAG",      // STAG Industrial
                    "ADC",       // Agree Realty Corporation
                    "BEN",       // Franklin Resources, Inc.
                    "AMCR",      // Amcor Plc
                    "TROW",      // T. Rowe Price Group Inc.
                    "FRT",       // Federal Realty Investment Trust
                    "IBM",       // International Business Machines Corp.
                    "ABBV",      // Abbvie Inc
                    "CVX",       // Chevron Corp.
                    "KVUE",      // Kenvue Inc
                    "ESS",       // Essex Property Trust, Inc.
                    "SJM",       // J.M. Smucker Co.
                    "KMB",       // Kimberly-Clark Corp.
                    "SWK",       // Stanley Black & Decker Inc
                    "ED",        // Consolidated Edison, Inc.
                    "CLX",       // Clorox Co.
                    "JNJ",       // Johnson & Johnson
                    "MDT",       // Medtronic Plc
                    "ADM",       // Archer Daniels Midland Co.
                    "HRL",       // Hormel Foods Corp.
                    "XOM",       // Exxon Mobil Corp.
                    "KO",        // Coca-Cola Co
                    "CHRW",      // C.H. Robinson Worldwide, Inc.
                    "PEP",       // PepsiCo Inc
                    "APD",       // Air Products & Chemicals Inc.
                    "NEE",       // NextEra Energy Inc
                    "CINF",      // Cincinnati Financial Corp.
                    "SYY",       // Sysco Corp.
                    "TGT",       // Target Corp
                    "ATO",       // Atmos Energy Corp.
                    "GPC",       // Genuine Parts Co.
                    "MCD",       // McDonald's Corp
                    "PG",        // Procter & Gamble Co.
                    "AFL",       // Aflac Inc.
                    "ADP",       // Automatic Data Processing Inc.
                    "FAST",      // Fastenal Co.
                    "ITW",       // Illinois Tool Works, Inc.
                    "MKC",       // McCormick & Co., Inc.
                    "ABT",       // Abbott Laboratories
                    "CL",        // Colgate-Palmolive Co.
                    "CAH",       // Cardinal Health, Inc.
                    "GD",        // General Dynamics Corp.
                    "LOW",       // Lowe's Cos., Inc.
                    "PPG",       // PPG Industries, Inc.
                    "BF.B",      // Brown-Forman Corp.
                    "EMR",       // Emerson Electric Co.
                    "BDX",       // Becton Dickinson & Co.
                    "AOS",       // A.O. Smith Corp.
                    "CAT",       // Caterpillar Inc.
                    "WMT",       // Walmart Inc
                    "CB",        // Chubb Limited
                    "LIN",       // Linde Plc.
                    "NUE",       // Nucor Corp.
                    "ALB",       // Albemarle Corp.
                    "EXPD",      // Expeditors International Of Washington, Inc.
                    "CHD",       // Church & Dwight Co., Inc.
                    "DOV",       // Dover Corp.
                    "PNR",       // Pentair plc
                    "ECL",       // Ecolab, Inc.
                    "NDSN",      // Nordson Corp.
                    "SHW",       // Sherwin-Williams Co.
                    "GWW",       // W.W. Grainger Inc.
                    "CTAS",      // Cintas Corporation
                    "SPGI",      // S&P Global Inc
                    "BRO",       // Brown & Brown, Inc.
                    "ROP",       // Roper Technologies Inc
                    "WST",        // West Pharmaceutical Services, Inc.
                    "NOW"        // ServiceNow 
                ];

            List<string> semiconductors =
                [
                    // IP and R&D
                    "SNPS",    // Synopsys
                    "CDNS",    // Cadence
                    "QCOM",    // Qualcomm
                    "ANSS",    // Ansys
                    "KEYS",    // Keysight Technologies
                    "SIEGY",   // Siemens
                    "CEVA",    // CEVA
                    "RMBS",    // Rambus                
                    // Equipment & Components
                    "ASMIY",   // ASM International
                    "ASML",    // ASML
                    "AIXA.DE", // Aixtron
                    "CAJ",     // Canon
                    "HTHIY",   // Hitachi
                    "ACMR",    // ACM Research
                    "LRCX",    // Lam Research
                    "KLAC",    // KLA
                    "AMAT",    // Applied Materials
                    "TOELY",   // Tokyo Electron
                    "NINOY",   // Nikon
                    "BESI.AS", // Besi                
                    // IDM (Integrated Device Manufacturer)
                    "INTC",    // Intel
                    "MU",      // Micron
                    "QRVO",    // Qorvo
                    "ON",      // onsemi
                    "WOLF",    // Wolfspeed
                    "TXN",     // Texas Instruments
                    "MCHP",    // Microchip
                    "ADI",     // Analog Devices
                    "SWKS",    // Skyworks
                    "IFNNY",   // Infineon
                    "NXPI",    // NXP
                    "STM",     // STMicroelectronics
                    "TOSYY",   // Toshiba
                    "RNECY",   // Renesas
                    "MRAAY",   // Murata
                    "SSNLF",   // Samsung
                    "HXSCL",   // SK hynix                
                    // Fabless Design
                    "NVDA",    // Nvidia
                    "AAPL",    // Apple
                    "CSCO",    // Cisco
                    "AVGO",    // Broadcom
                    "MPWR",    // Monolithic Power Systems
                    "AMD",     // AMD
                    "GOOG",    // Alphabet (Google) Class C
                    "GOOGL",   // Alphabet (Google) Class A
                    "MRVL",    // Marvell
                    "META",    // Meta (Facebook)
                    "CRUS",    // Cirrus Logic                
                    // Foundries
                    "TSM",     // TSMC
                    "UMC",     // UMC
                    "TSEM",    // Tower Semiconductor
                    "GFS",     // GlobalFoundries
                    "SSNLF"    // Samsung (duplicate)
                ];

            List<string> stocksWithHighProfitMargin =
                [
                    "FANG", "TSM", "TRI", "WPC", "AER", "UTHR", "PSA", "KSPI", "MA",
                    "BAM", "MSCI", "EWBC", "RPRX", "ABNB", "LNG", "NVDA", "GLPI", "WPM", "BCH",
                    "V", "VRSN", "CME", "ARCC", "TPL", "FCNCA", "EMR", "VICI", "MSTR",
                    "CI", "DIS", "ELV", "BRK.B", "HD", "UPS", "NKE", "XOM", "CVX",
                    "TSLA", "VZ", "WFC", "JNJ", "META", "GOOG", "BAC", "MRK",
                    "JPM", "PFE", "MSFT"
                ];

            var allSymbols = stocksWithHighProfitMargin
                .Concat(tenStocksToOwnForever)
                .Concat(stockTickers)
                .Concat(etfTickers)
                .Concat(dividendAristocrats)
                .Concat(semiconductors)
                .ToList();

            allSymbols = allSymbols.Select(symbol => symbol.Trim().ToUpperInvariant())
                                   .Distinct(StringComparer.InvariantCultureIgnoreCase)
                                   .ToList();

            var fetchedAssets = await mediator.Send(new GetAssetsQuery());
            var fetchedAssetNames = fetchedAssets.ConvertAll(f => f.Symbol);

            bool allStockSymbolsExist = true;
            foreach (var symbol in allSymbols)
            {
                var assetFound = fetchedAssetNames.Find(name => string.Equals(name, symbol, StringComparison.InvariantCultureIgnoreCase));
                if (assetFound is null)
                {
                    allStockSymbolsExist = false;
                    break;
                }
            }

            if (alpacaProvider.IsProviderAssetPairsSyncedToday() && alpacaProvider.IsProviderCandlesticksSyncedToday(timeframe) && allStockSymbolsExist)
            {
                logger.LogInformation("{Provider} synchronized for today", provider);
                return true;
            }

            await SyncAssets(fetchedAssets, allSymbols);
            await SyncPairs(allSymbols);
            var candlesticksUpdated = await SyncCandlesticks(timeframe);

            if (!candlesticksUpdated)
            {
                return false;
            }

            alpacaProvider.ProviderPairAssetSyncInfo.UpdateProviderInfo();
            var providerCandlestickSyncInfo = alpacaProvider.GetOrCreateProviderCandlestickSyncInfo(provider, timeframe);
            await mediator.Send(new UpdateExchangeCommand(alpacaProvider.ProviderPairAssetSyncInfo, providerCandlestickSyncInfo));
            return true;
        }

        private async Task SyncAssets(List<Asset> fetchedAssets, List<string> stockSymbols)
        {
            var existingAssetNames = fetchedAssets.Select(a => a.Symbol).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            List<Asset> newAssets = [];
            foreach (var stockSymbol in stockSymbols)
            {
                if (!existingAssetNames.Contains(stockSymbol, StringComparer.InvariantCultureIgnoreCase))
                {
                    if (etfTickers.Contains(stockSymbol, StringComparer.InvariantCultureIgnoreCase))
                    {
                        Asset newAsset = new() { Symbol = stockSymbol, ProductType = ProductType.ETF };
                        newAssets.Add(newAsset);
                    }
                    else
                    {
                        Asset newAsset = new() { Symbol = stockSymbol, ProductType = ProductType.Stock };
                        newAssets.Add(newAsset);
                    }

                }
            }

            if (newAssets.Count > 0)
            {
                await mediator.Send(new InsertAssetsCommand(newAssets));
            }
        }

        public async Task SyncPairs(List<string> stockSymbols)
        {
            var fetchedAssetsTask = mediator.Send(new GetAssetsQuery());
            var fetchedPairsTask = mediator.Send(new GetPairsQuery());

            var assets = (await fetchedAssetsTask).ToList();
            var pairs = (await fetchedPairsTask).Where(fp => fp.Provider == DataProvider.Alpaca).ToList();

            var newPairs = new List<PairExtended>();
            foreach (var stockSymbol in stockSymbols)
            {
                var baseAsset = assets.Find(a => string.Equals(a.Symbol, stockSymbol, StringComparison.OrdinalIgnoreCase));

                if (baseAsset is null)
                {
                    return;
                }

                var pairExists = pairs.Find(fp => fp.BaseAssetId == baseAsset?.PrimaryId);

                if (pairExists is null)
                {
                    PairExtended newPair = new()
                    {
                        BaseAssetId = baseAsset.PrimaryId,
                        BaseAssetName = baseAsset.Symbol,
                        Provider = DataProvider.Alpaca,
                        Symbol = baseAsset.Symbol,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                    };
                    newPairs.Add(newPair);
                }
            }

            if (newPairs.Count > 0)
            {
                await mediator.Send(new InsertPairsCommand(newPairs));
            }
        }

        private async Task<bool> SyncCandlesticks(Timeframe period = Timeframe.Daily)
        {
            var fetchedAssetsTask = mediator.Send(new GetAssetsQuery());
            var fetchedPairsTask = mediator.Send(new GetPairsQuery());
            var fetchedCandlesticksTask = mediator.Send(new GetCandlesticksQuery());

            var fetchedAssets = await fetchedAssetsTask;
            var fetchedPairs = await fetchedPairsTask;
            var fetchedCandlesticks = await fetchedCandlesticksTask;

            var pairs = fetchedPairs.Where(p => p.Provider == DataProvider.Alpaca).ToList();

            pairs.MapPairsToAssets(fetchedAssets);
            pairs.MapPairsToCandlesticks(fetchedCandlesticks);

            var newCandlesticks = new List<CandlestickExtended>();

            foreach (var fetchedPair in pairs)
            {
                var latestCandlestickOpenTime = fetchedPair.Candlesticks
                                             .Select(candlestick => candlestick.OpenDate)
                                             .DefaultIfEmpty(DateTime.MinValue)
                                             .Max();

                DateTime fromDatetime = latestCandlestickOpenTime.AddDays(1);
                DateTime toDatetime = default;

                var yesterday = DateTime.UtcNow.AddDays(-1).Date;

                if (fromDatetime > yesterday.Date)
                {
                    continue;
                }

                if (latestCandlestickOpenTime != default)
                {
                    toDatetime = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                }

                if (latestCandlestickOpenTime == default && (period == Timeframe.Daily || period == Timeframe.Weekly))
                {
                    fromDatetime = DateTime.UtcNow.AddYears(-10).AddDays(1).Date;
                    toDatetime = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);
                }
                else if (latestCandlestickOpenTime == default && period == Timeframe.OneHour)
                {
                    fromDatetime = DateTime.UtcNow.AddHours(1).Date.AddHours(DateTime.UtcNow.AddHours(1).Hour).AddYears(-10);
                    toDatetime = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour - 1).AddMinutes(59).AddSeconds(59);
                }

                foreach (var dateRange in DatetimeExtension.GetDailyDateRanges(fromDatetime, toDatetime))
                {
                    if (fetchedPair.BaseAssetName != null)
                    {
                        var stockData = await alpacaHttpClient.GetAlpacaData(fetchedPair.BaseAssetName, dateRange.Item1, dateRange.Item2, BarTimeFrame.Day);
                        if (stockData.HasError)
                        {
                            return false;
                        }

                        foreach (var kvp in stockData.SuccessValue.Items)
                        {
                            foreach (var bar in kvp.Value)
                            {
                                var candlestick = new CandlestickBuilder()
                                    .WithPoolOrPairId(fetchedPair.PrimaryId)
                                    .WithPoolOrPairName(kvp.Key)
                                    .WithCloseDate(bar.TimeUtc)
                                    .WithOpenDate(bar.TimeUtc.Date)
                                    .WithOpenPrice(bar.Open)
                                    .WithHighPrice(bar.High)
                                    .WithLowPrice(bar.Low)
                                    .WithClosePrice(bar.Close)
                                    .WithTimeframe(Timeframe.Daily)
                                    .WithVolume(bar.Volume)
                                    .Build();

                                newCandlesticks.Add(candlestick);
                            }
                        }
                    }
                }
            }

            if (newCandlesticks.Count > 0)
            {
                await mediator.Send(new InsertCandlesticksCommand(newCandlesticks));
            }

            return true;
        }
    }
}
