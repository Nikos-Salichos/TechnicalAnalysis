using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Infrastructure.Persistence.Repositories;
using Testcontainers.PostgreSql;

namespace TechnicalAnalysis.Tests.IntegrationTests.TestContainers.BaseClasses
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public PostgreSqlContainer PostgreSqlContainer;

        public IntegrationTestWebAppFactory()
        {
            PostgreSqlContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithUsername("postgres")
                .WithPassword("admin")
                .WithDatabase("TechnicalAnalysisTest")
                .WithCleanUp(true)
                .WithAutoRemove(true)
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("IntegrationTest");

            var solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            var basePath = Path.Combine(solutionDirectory, "TechnicalAnalysis.Infrastructure.Host");

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json");

            var config = configBuilder.Build();
            var apiKey = config["ApiKey"];

            var connectionString = PostgreSqlContainer.GetConnectionString();

            builder.ConfigureTestServices(services =>
            {
                // Register the database configuration
                var configurationBuilder = new ConfigurationBuilder();

                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"ConnectionStrings:PostgreSqlTechnicalAnalysisDockerCompose", connectionString},
                });

                var configuration = configurationBuilder.Build();

                configuration.GetSection("ApiKey").Value = apiKey;

                services.AddSingleton<IConfiguration>(configuration);
                services.AddTransient<IPostgreSqlRepository, PostgreSqlRepository>();

                // Add options for database settings
                services.Configure<DatabaseSetting>(configuration.GetSection("ConnectionStrings"));
            });
        }

        public async Task InitializeAsync()
        {
            await PostgreSqlContainer.StartAsync();
            await InitializeDatabaseScriptAsync();
        }

        public async Task InitializeDatabaseScriptAsync()
        {
            await using var dbConnection = new NpgsqlConnection(PostgreSqlContainer.GetConnectionString());
            await dbConnection.OpenAsync();

            var scriptPath = Path.Combine(AppContext.BaseDirectory, "createTables.sql");
            var script = await File.ReadAllTextAsync(scriptPath);

            // Combine all the SQL queries into one transaction
            await using var transaction = await dbConnection.BeginTransactionAsync();
            await dbConnection.ExecuteAsync(script, transaction: transaction);

            // Commit the transaction to execute all queries
            await transaction.CommitAsync();
        }

        public new Task DisposeAsync()
        {
            return PostgreSqlContainer.DisposeAsync().AsTask();
        }
    }
}
