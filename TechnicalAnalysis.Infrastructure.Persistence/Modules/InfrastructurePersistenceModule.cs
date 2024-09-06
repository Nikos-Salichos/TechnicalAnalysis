using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Infrastructure.Persistence.Repositories;
using TechnicalAnalysis.Infrastructure.Persistence.SqlHandlers;

namespace TechnicalAnalysis.Infrastructure.Persistence.Modules
{
    public static class InfrastructurePersistenceModule
    {
        public static void AddInfrastructurePersistenceModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPostgreSqlRepository, PostgreSqlRepository>();
            services.AddSingleton<IHealthCheckRepository, HealthCheckRepository>();
            services.AddSingleton<IRedisRepository, RedisRepository>();

            // Register ConnectionMultiplexer as a singleton
            services.AddSingleton(_ =>
            {
                var options = new ConfigurationOptions
                {
                    EndPoints =
                    {
                        configuration["ConnectionStrings:RedisDockerCompose"] 
                        ?? throw new InvalidOperationException("ConnectionStrings:RedisDockerCompose is null")
                    },
                    AbortOnConnectFail = false,
                    ConnectTimeout = 5,
                    ConnectRetry = 10,
                };
                return ConnectionMultiplexer.Connect(options);
            });

            // Register IDistributedCache as a singleton
            services.AddSingleton<IDistributedCache>(c =>
            {
                var connectionMultiplexer = c.GetRequiredService<ConnectionMultiplexer>();
                return new RedisCache(new RedisCacheOptions
                {
                    Configuration = connectionMultiplexer.Configuration,
                    InstanceName = "TechnicalAnalysis"
                });
            });

            services.AddOptions<DatabaseSetting>().Bind(configuration.GetSection("ConnectionStrings")).ValidateDataAnnotations();

            SqlMapper.AddTypeHandler(new ValueClassificationTypeHandler());
        }
    }
}