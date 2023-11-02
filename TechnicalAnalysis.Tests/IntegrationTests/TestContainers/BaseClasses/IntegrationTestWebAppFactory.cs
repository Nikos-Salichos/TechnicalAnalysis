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
                .WithName("postgresql-testcontainer")
                .WithUsername("postgres")
                .WithPassword("admin")
                .WithDatabase("TechnicalAnalysisTest")
                .WithCleanUp(true)
                .WithAutoRemove(true)
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var connectionString = PostgreSqlContainer.GetConnectionString();

            builder.ConfigureTestServices(services =>
            {
                // Register the database configuration
                var configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"ConnectionStrings:PostgreSqlTechnicalAnalysisDockerCompose", connectionString}
                });
                var configuration = configurationBuilder.Build();

                services.AddSingleton<IConfiguration>(configuration);

                // Register the repository
                services.AddTransient<IPostgreSqlRepository, PostgreSqlRepository>();

                // Add options for database settings
                services.Configure<DatabaseSetting>(configuration.GetSection("ConnectionStrings"));

                // If you need to replace an existing DbContext, you can do it here
                /*
                var descriptorType = typeof(DbContextOptions<ApplicationDbContext);
                var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString()));
                */
            });

        }

        public async Task InitializeAsync()
        {
            await PostgreSqlContainer.StartAsync();

            using var dbConnection = new NpgsqlConnection(PostgreSqlContainer.GetConnectionString());
            await dbConnection.OpenAsync();

            const string query = @"
                                CREATE TABLE public.""Assets"" (
                                    ""Id"" bigint NOT NULL,
                                    ""Symbol"" text UNIQUE,
                                    ""CreatedDate"" date
                                );
                                
                                ALTER TABLE public.""Assets"" OWNER TO postgres;
                                
                                ALTER TABLE public.""Assets"" ALTER COLUMN ""Id"" ADD GENERATED ALWAYS AS IDENTITY (
                                    SEQUENCE NAME public.""Assets_Id_seq""
                                    START WITH 1
                                    INCREMENT BY 1
                                    NO MINVALUE
                                    NO MAXVALUE
                                    CACHE 1
                                );";

            // Combine all the SQL queries into one transaction
            using var transaction = dbConnection.BeginTransaction();
            await dbConnection.ExecuteAsync(query, transaction: transaction);

            // Commit the transaction to execute all queries
            transaction.Commit();
        }


        public new Task DisposeAsync()
        {
            return PostgreSqlContainer.DisposeAsync().AsTask();
        }
    }
}
