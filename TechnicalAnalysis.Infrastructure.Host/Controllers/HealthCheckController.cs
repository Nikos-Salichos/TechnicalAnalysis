using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Host.Controllers
{
    [Route("api/healthcheck")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHealthCheckRepository healthCheckRepository;

        public HealthCheckController(IHealthCheckRepository healthCheckService)
        {
            healthCheckRepository = healthCheckService;
        }

        [HttpGet("CheckPostgreSqlHealth")]
        public async Task<IActionResult> CheckHealthAsync()
        {
            HealthCheckContext healthCheckContext = new();
            var report = await healthCheckRepository.CheckHealthAsync(healthCheckContext);

            return report.Status == HealthStatus.Healthy ? Ok(report) :
                StatusCode((int)HttpStatusCode.ServiceUnavailable, report);
        }
    }
}
