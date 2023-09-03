using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IHealthCheckRepository
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
    }
}
