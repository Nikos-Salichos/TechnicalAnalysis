using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnicalAnalysis.Domain.Settings;

namespace TechnicalAnalysis.Domain.Modules
{
    public static class DomainModule
    {
        public static void AddDomainModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AlpacaSetting>().Bind(configuration.GetSection("AlpacaSettings")).ValidateDataAnnotations();
            services.AddOptions<BinanceSetting>().Bind(configuration.GetSection("BinanceSettings")).ValidateDataAnnotations();
            services.AddOptions<DexSetting>().Bind(configuration.GetSection("DexSettings")).ValidateDataAnnotations();
            services.AddOptions<CoinMarketCapSetting>().Bind(configuration.GetSection("CoinMarketCap")).ValidateDataAnnotations();
            services.AddOptions<CoinRankingSetting>().Bind(configuration.GetSection("CoinRanking")).ValidateDataAnnotations();
            services.AddOptions<RapidApiSetting>().Bind(configuration.GetSection("RapidApi")).ValidateDataAnnotations();
            services.AddOptions<CoinPaprikaSetting>().Bind(configuration.GetSection("CoinPaprika")).ValidateDataAnnotations();
            services.AddOptions<CnnApiSetting>().Bind(configuration.GetSection("Cnn")).ValidateDataAnnotations();
            services.AddOptions<FredApiSetting>().Bind(configuration.GetSection("FredApi")).ValidateDataAnnotations();
        }
    }
}
