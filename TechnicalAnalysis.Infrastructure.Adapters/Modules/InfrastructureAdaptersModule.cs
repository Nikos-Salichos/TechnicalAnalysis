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
using TechnicalAnalysis.Infrastructure.Adapters.RabbitMQ;

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
                    _ => throw new ArgumentOutOfRangeException(nameof(provider), $"Exchange {provider} not found")
                };
            });

            services.AddHttpClient("default")
                .ConfigureHttpClient(client => client.DefaultRequestHeaders.Add("User-Agent", "Tracking prices application"));

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

            services.AddOptions<MailSettings>().Bind(configuration.GetSection(nameof(MailSettings)));
            services.AddOptions<RabbitMqSetting>().Bind(configuration.GetSection("RabbitMq"));

            services.AddSingleton<IMailer, Mailer>();
            services.AddSingleton<ICommunication, Communication>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
        }
    }
}
