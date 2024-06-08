using Microsoft.Extensions.DependencyInjection;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Tests.IntegrationTests.BaseClasses
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        private bool _disposed;
        private readonly IServiceScope _serviceScope;
        protected readonly IPostgreSqlRepository PostgreSqlRepository;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            _serviceScope = factory.Services.CreateScope();
            PostgreSqlRepository = _serviceScope.ServiceProvider.GetRequiredService<IPostgreSqlRepository>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here
                    _serviceScope?.Dispose();
                }

                // Dispose unmanaged resources here

                _disposed = true;
            }
        }

        ~BaseIntegrationTest()
        {
            Dispose(false);
        }
    }
}
