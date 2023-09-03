using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Npgsql;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Settings;

namespace TechnicalAnalysis.Infrastructure.Persistence.Repositories
{
    public class HealthCheckRepository : IHealthCheck, IHealthCheckRepository
    {
        private readonly string _connectionStringKey;

        public HealthCheckRepository(IOptionsMonitor<DatabaseSetting> databaseSettings)
        {
            _connectionStringKey = databaseSettings.CurrentValue.PostgreSqlTechnicalAnalysisDockerCompose;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using var connection = new NpgsqlConnection(_connectionStringKey);
            try
            {
                await connection.OpenAsync();
                return HealthCheckResult.Healthy();
            }
            catch (Exception)
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
