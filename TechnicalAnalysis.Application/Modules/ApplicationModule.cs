using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TechnicalAnalysis.Application.Services;
using TechnicalAnalysis.Application.Validations;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Settings;

namespace TechnicalAnalysis.Application.Modules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<AnalysisService>();  // Register the original service
            // Register the decorator
            services.AddSingleton<IAnalysisService>(serviceProvider =>
            {
                var analysisService = serviceProvider.GetRequiredService<AnalysisService>();
                var redisRepository = serviceProvider.GetRequiredService<IRedisRepository>();
                var communicationService = serviceProvider.GetRequiredService<ICommunication>();
                var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
                return new CachedAnalysisService(analysisService, redisRepository, communicationService, rabbitMqService);
            });

            services.AddSingleton<ISyncService, SyncService>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            //Use in IOptionsMonitor<>
            services.AddOptions<AlpacaSetting>().Bind(configuration.GetSection("AlpacaSettings"));
            services.AddOptions<BinanceSetting>().Bind(configuration.GetSection("BinanceSettings"));
            services.AddOptions<DexSetting>().Bind(configuration.GetSection("DexSettings"));

            services.AddSingleton<IValidator<DataProviderTimeframeRequest>, DataProviderTimeframeValidator>();
        }
    }
}
