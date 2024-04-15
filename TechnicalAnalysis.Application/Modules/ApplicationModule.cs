using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TechnicalAnalysis.Application.Services;
using TechnicalAnalysis.Application.Validations;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.Domain.Interfaces.Application;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Application.Modules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModule(this IServiceCollection services)
        {
            // Register the original service
            services.AddSingleton<AnalysisService>();

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

            services.AddSingleton<IValidator<DataProviderTimeframeRequest>, DataProviderTimeframeValidator>();
        }
    }
}
