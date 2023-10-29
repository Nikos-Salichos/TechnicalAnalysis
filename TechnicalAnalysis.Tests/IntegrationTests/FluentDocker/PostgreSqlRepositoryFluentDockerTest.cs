using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Infrastructure.Persistence.Repositories;

namespace TechnicalAnalysis.Tests.IntegrationTests.FluentDocker
{
    public class PostgreSqlRepositoryFluentDockerTest
    {
        public IPostgreSqlRepository PostgreSqlRepository;

        public PostgreSqlRepositoryFluentDockerTest()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"ConnectionStrings:PostgreSqlTechnicalAnalysisDockerCompose",
                    "Host=host.docker.internal; Port=5432; Database=TechnicalAnalysis; Username=postgres; Password=admin"}
                    });
            var configuration = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IPostgreSqlRepository, PostgreSqlRepository>();
            serviceCollection.AddOptions<DatabaseSetting>().Bind(configuration.GetSection("ConnectionStrings"));
            serviceCollection.AddLogging(builder => builder.AddConsole().AddDebug());

            var provider = serviceCollection.BuildServiceProvider();
            PostgreSqlRepository = provider.GetRequiredService<IPostgreSqlRepository>();
        }

        [Fact]
        public async Task ExecuteAssetsCommand_Successful()
        {
            await PostgreSqlRepository.DeleteAssetsAsync();

            List<Asset> assets = new List<Asset>
            {
                new Asset { Symbol = "TestContainersAsset"},
            };

            await PostgreSqlRepository.InsertAssetsAsync(assets);

            var retrievedAssets = await PostgreSqlRepository.GetAssetsAsync();
            retrievedAssets.SuccessValue.Should().NotBeNull();
            retrievedAssets.FailValue.Should().BeNull();
            retrievedAssets.SuccessValue.First().Symbol.Should().Be(assets[0].Symbol);
        }


    }
}
