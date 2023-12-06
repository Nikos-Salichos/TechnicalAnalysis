using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Host.Controllers
{
    [Route("api/healthcheck")]
    [ApiController]
    public class HealthCheckController(IHealthCheckRepository healthCheckService, ILogger<HealthCheckController> logger) : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger = logger;

        [HttpGet("CheckPostgreSqlHealth")]
        public async Task<IActionResult> CheckHealthAsync()
        {
            HealthCheckContext healthCheckContext = new();
            var report = await healthCheckService.CheckHealthAsync(healthCheckContext);

            return report.Status == HealthStatus.Healthy
                ? Ok(report)
                : StatusCode((int)HttpStatusCode.ServiceUnavailable, report);
        }
    }
}
