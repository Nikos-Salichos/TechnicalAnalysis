using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Npgsql;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Settings;

namespace TechnicalAnalysis.Infrastructure.Persistence.Repositories
{
    public class HealthCheckRepository(IOptionsMonitor<DatabaseSetting> databaseSettings) : IHealthCheck, IHealthCheckRepository
    {
        private readonly string _connectionStringKey = databaseSettings.CurrentValue.PostgreSqlTechnicalAnalysisDockerCompose;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            await using var connection = new NpgsqlConnection(_connectionStringKey);
            await connection.OpenAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
    }
}
