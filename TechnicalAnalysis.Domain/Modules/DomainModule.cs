using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnicalAnalysis.Domain.Settings;

namespace TechnicalAnalysis.Domain.Modules
{
    public static class DomainModule
    {
        public static void AddDomainModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AlpacaSetting>().Bind(configuration.GetSection("AlpacaSettings"));
            services.AddOptions<BinanceSetting>().Bind(configuration.GetSection("BinanceSettings"));
            services.AddOptions<DexSetting>().Bind(configuration.GetSection("DexSettings"));
            services.AddOptions<CoinMarketCapSetting>().Bind(configuration.GetSection("CoinMarketCap"));
            services.AddOptions<CoinRankingSetting>().Bind(configuration.GetSection("CoinRanking"));
            services.AddOptions<RapidApiSetting>().Bind(configuration.GetSection("RapidApi"));
            services.AddOptions<CoinPaprikaSetting>().Bind(configuration.GetSection("CoinPaprika"));
            services.AddOptions<CnnApiSetting>().Bind(configuration.GetSection("Cnn"));
        }
    }
}
