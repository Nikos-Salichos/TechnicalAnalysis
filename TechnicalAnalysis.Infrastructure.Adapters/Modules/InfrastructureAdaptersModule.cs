using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Interfaces;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
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
            services.AddCors(options => options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.AddSingleton<BinanceAdapter>();
            services.AddSingleton<AlpacaAdapter>();
            services.AddSingleton<DexV3Adapter>();
            services.AddSingleton<WallStreetZenAdapter>();

            services.AddSingleton<Func<Provider, IAdapter>>(serviceProvider => provider =>
            {
                IAdapter adapter = provider switch
                {
                    Provider.Binance => serviceProvider.GetService<BinanceAdapter>(),
                    Provider.Alpaca => serviceProvider.GetService<AlpacaAdapter>(),
                    Provider.Uniswap or Provider.Pancakeswap => serviceProvider.GetService<DexV3Adapter>(),
                    Provider.WallStreetZen => serviceProvider.GetService<WallStreetZenAdapter>(),
                    Provider.All => throw new NotImplementedException($"Exchange {provider} has not been implemented found"),
                    _ => throw new ArgumentOutOfRangeException($"Exchange {provider} not found")
                };

                return adapter ?? throw new InvalidOperationException($"Could not resolve adapter for {provider}");
            });

            services.AddHttpClient(); // Register IHttpClientFactory
            services.AddSingleton<IBinanceHttpClient, BinanceHttpClient>();
            services.AddSingleton<IDexV3HttpClient, DexV3HttpClient>();
            services.AddSingleton<IAlpacaHttpClient, AlpacaHttpClient>();
            services.AddSingleton<IWallStreetZenClient, WallStreetZenClient>();
            services.AddSingleton<IPollyPolicy, PollyPolicy>();

            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            var mailSettings = configuration.GetSection(nameof(MailSettings)).Get<MailSettings>();

            services.AddOptions<RabbitMqSetting>().Bind(configuration.GetSection("RabbitMq"));

            services.AddSingleton<IMailer, Mailer>();
            services.AddSingleton<ICommunication, Communication>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
        }
    }
}
