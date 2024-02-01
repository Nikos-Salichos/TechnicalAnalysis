using Microsoft.Extensions.DependencyInjection;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Tests.IntegrationTests.TestContainers.BaseClasses
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        private readonly IServiceScope _serviceScope;
        protected readonly IPostgreSqlRepository PostgreSqlRepository;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _serviceScope = factory.Services.CreateScope();
            PostgreSqlRepository = _serviceScope.ServiceProvider.GetRequiredService<IPostgreSqlRepository>();
        }

        public void Dispose()
        {
            _serviceScope?.Dispose();
        }
    }
}
