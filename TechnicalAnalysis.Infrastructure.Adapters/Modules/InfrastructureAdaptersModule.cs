using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Messages;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;
using TechnicalAnalysis.Infrastructure.Adapters.Adapters;
using TechnicalAnalysis.Infrastructure.Adapters.HttpClients;
using TechnicalAnalysis.Infrastructure.Adapters.MessageBrokers;

namespace TechnicalAnalysis.Infrastructure.Adapters.Modules
{
    public static class InfrastructureAdaptersModule
    {
        public static void AddInfrastructureAdapterModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<BinanceAdapter>();
            services.AddSingleton<AlpacaAdapter>();
            services.AddSingleton<DexV3Adapter>();
            services.AddSingleton<WallStreetZenAdapter>();
            services.AddSingleton<CryptoFearAndGreedAdapter>();
            services.AddSingleton<CoinPaprikaAdapter>();
            services.AddSingleton<CoinMarketCapAdapter>();
            services.AddSingleton<CoinRankingAdapter>();
            services.AddSingleton<StockFearAndGreedAdapter>();
            services.AddSingleton<CnnFearAndGreedAdapter>();

            services.AddSingleton<Func<DataProvider, IAdapter>>(serviceProvider => provider =>
            {
                return provider switch
                {
                    DataProvider.Binance => serviceProvider.GetRequiredService<BinanceAdapter>(),
                    DataProvider.Alpaca => serviceProvider.GetRequiredService<AlpacaAdapter>(),
                    DataProvider.Uniswap or DataProvider.Pancakeswap => serviceProvider.GetRequiredService<DexV3Adapter>(),
                    DataProvider.WallStreetZen => serviceProvider.GetRequiredService<WallStreetZenAdapter>(),
                    DataProvider.AlternativeMeCryptoFearAndGreedProvider => serviceProvider.GetRequiredService<CryptoFearAndGreedAdapter>(),
                    DataProvider.CoinPaprika => serviceProvider.GetRequiredService<CoinPaprikaAdapter>(),
                    DataProvider.CoinMarketCap => serviceProvider.GetRequiredService<CoinMarketCapAdapter>(),
                    DataProvider.CoinRanking => serviceProvider.GetRequiredService<CoinRankingAdapter>(),
                    DataProvider.RapidApiStockFearAndGreedProvider => serviceProvider.GetRequiredService<StockFearAndGreedAdapter>(),
                    DataProvider.CnnApiStockFearAndGreedProvider => serviceProvider.GetRequiredService<CnnFearAndGreedAdapter>(),
                    _ => throw new ArgumentOutOfRangeException(nameof(provider), $"Exchange {provider} not found")
                };
            });

            services.AddLogging();

            services.AddHttpClient("default")
                    .ConfigureHttpClient(client => client.DefaultRequestHeaders.Add("User-Agent", "Tracking prices application"));

            //Headers required by CNN Api
            services.AddHttpClient("cnn")
                .ConfigureHttpClient(client =>
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
                    client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,el;q=0.8");
                    client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                    client.DefaultRequestHeaders.Add("DNT", "1");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
                    client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                    client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                    client.DefaultRequestHeaders.Add("User-Agent", "Tracking prices application");
                });

            services.AddSingleton<IPollyPolicy, PollyPolicy>();
            services.AddSingleton<IBinanceHttpClient, BinanceHttpClient>();
            services.AddSingleton<IDexV3HttpClient, DexV3HttpClient>();
            services.AddSingleton<IAlpacaHttpClient, AlpacaHttpClient>();
            services.AddSingleton<IWallStreetZenClient, WallStreetZenHttpClient>();
            services.AddSingleton<ICryptoFearAndGreedHttpClient, CryptoFearAndGreedHttpClient>();
            services.AddSingleton<ICoinPaprikaHttpClient, CoinPaprikaHttpClient>();
            services.AddSingleton<ICoinMarketCapHttpClient, CoinMarketCapHttpClient>();
            services.AddSingleton<ICoinRankingHttpClient, CoinRankingHttpClient>();
            services.AddSingleton<IStockFearAndGreedHttpClient, StockFearAndGreedHttpClient>();
            services.AddSingleton<ICnnStockFearAndGreedHttpClient, CnnStockFearAndGreedHttpClient>();

            services.AddOptions<MailSettings>().Bind(configuration.GetSection(nameof(MailSettings))).ValidateDataAnnotations();
            services.AddOptions<RabbitMqSetting>().Bind(configuration.GetSection("RabbitMq"));

            services.AddSingleton<IMailer, Mailer>();
            services.AddSingleton<ICommunication, Communication>();
        }
    }
}
