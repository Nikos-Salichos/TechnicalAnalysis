using Microsoft.Extensions.DependencyInjection;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Tests.IntegrationTests.TestContainers.BaseClasses
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        private readonly IServiceScope _scope;
        protected readonly IPostgreSqlRepository PostgreSqlRepository;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();
            PostgreSqlRepository = _scope.ServiceProvider.GetRequiredService<IPostgreSqlRepository>();
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
