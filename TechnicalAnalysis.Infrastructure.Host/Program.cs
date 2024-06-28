using Hangfire;
using Hangfire.PostgreSql;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;
using TechnicalAnalysis.Application.Modules;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Modules;
using TechnicalAnalysis.Infrastructure.Adapters.Modules;
using TechnicalAnalysis.Infrastructure.Host.Hangfire;
using TechnicalAnalysis.Infrastructure.Host.Middleware;
using TechnicalAnalysis.Infrastructure.Host.Modules;
using TechnicalAnalysis.Infrastructure.Host.RabbitMQ;
using TechnicalAnalysis.Infrastructure.Host.Serilog;
using TechnicalAnalysis.Infrastructure.Host.Services;
using TechnicalAnalysis.Infrastructure.Persistence.Modules;

var builder = WebApplication.CreateBuilder(args);

#region Read Configuration
if (builder.Environment.EnvironmentName == "IntegrationTest")
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("appsettings.prod.json", optional: false, reloadOnChange: true);
}
#endregion Read Configuration

#region Serilog
builder.SerilogConfiguration();
#endregion Serilog

#region Layer Modules
builder.Services.AddDomainModule(builder.Configuration);
builder.Services.AddInfrastructurePersistenceModule(builder.Configuration);
builder.Services.AddInfrastructureAdapterModule(builder.Configuration);
builder.Services.AddApplicationModule();
builder.Services.AddInfrastructureHostModule();
#endregion Layer Modules

builder.Services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);

#region Api Rate Limit
builder.Services.ConfigureRateLimit();
#endregion Api Rate Limit

if (builder.Environment.EnvironmentName != "IntegrationTest")
{
    builder.Services.AddHangfire(configuration =>
    {
        var hangfireConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        configuration.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(hangfireConnectionString));
    });
}

#region RabbitMq
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
#endregion RabbitMq

if (builder.Environment.EnvironmentName != "IntegrationTest")
{
    builder.Services.AddHangfireServer();
}

#region Open Telemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("TechnicalAnalysis"))
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddRuntimeInstrumentation();

        metrics.AddMeter("TechnicalAnalysis");

        metrics.AddOtlpExporter();
    })
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddNpgsql();

        tracing.AddOtlpExporter();
    });

builder.Logging.AddOpenTelemetry(options =>
{
    options.AddOtlpExporter();
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
});
#endregion  Open Telemetry

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseMetricServer(); //Starting the metrics exporter, will expose "/metrics"

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.DisplayRequestDuration();
});

app.UseExceptionHandler();

app.UseSerilogRequestLogging(options
  => options.EnrichDiagnosticContext = LogRequestEnricher.LogAdditionalInfo);

app.UseMiddleware<ApiKeyMiddleware>();

app.UseCors();

app.UseResponseCompression();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseRateLimiter();

app.UseMiddleware<SecureHeadersMiddleware>();

if (builder.Environment.EnvironmentName != "IntegrationTest")
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions()
    {
        Authorization = new[] { new DashboardNoAuthorizationFilter() }
    });

    //Note: Swagger will not run before jobs finish
    await HangfireStartupJob.EnqueueSynchronizeProvidersJob(app);
}

app.UseHttpMetrics(options =>
{
    options.CaptureMetricsUrl = true;
    options.RequestCount.Enabled = true;
    options.RequestDuration.Enabled = true;
    options.InProgress.Enabled = true;
    options.AddCustomLabel("host", context => context.Request.Host.Host);
});

app.UseAuthentication(); //first line should be

app.UseAuthorization(); //second line should be

app.MapControllers();

await app.RunAsync();

/// <summary>
/// I must declare Program partial class for testcontainers nuget (integration tests)
/// </summary>
public partial class Program { }