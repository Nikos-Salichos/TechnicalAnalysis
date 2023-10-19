using Testcontainers.PostgreSql;

namespace TechnicalAnalysis.Tests.IntegrationTests
{
    public class BaseIntegrationTest : IAsyncLifetime
    {
        public readonly PostgreSqlContainer PostgreSqlContainer;

        public BaseIntegrationTest()
        {
            PostgreSqlContainer = new PostgreSqlBuilder()
                                   //.WithPortBinding(5432)
                                   //.WithHostname("Host.docker.internal")
                                   .WithImage("postgres")
                                   .WithDatabase("TechnicalAnalysis")
                                   .WithUsername("postgres")
                                   .WithPassword("admin")
                                   //.WithBindMount("C:\\Users\\Nikos\\source\\repos\\TechnicalAnalysis\\createTables.sql", "/docker-entrypoint-initdb.d/createTables.sql")  // Mount the SQL script
                                   .WithCleanUp(true)
                                   .Build();
        }

        public Task InitializeAsync()
        {
            return PostgreSqlContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            return PostgreSqlContainer.DisposeAsync().AsTask();
        }
    }
}
