using Hangfire;
using Hangfire.PostgreSql;
using Serilog;
using TechnicalAnalysis.Application.Modules;
using TechnicalAnalysis.Domain.Modules;
using TechnicalAnalysis.Infrastructure.Adapters.Modules;
using TechnicalAnalysis.Infrastructure.Host.Hangfire;
using TechnicalAnalysis.Infrastructure.Host.Middleware;
using TechnicalAnalysis.Infrastructure.Host.Modules;
using TechnicalAnalysis.Infrastructure.Host.Serilog;
using TechnicalAnalysis.Infrastructure.Host.Services;
using TechnicalAnalysis.Infrastructure.Persistence.Modules;

var builder = WebApplication.CreateBuilder(args);

#region Read Configuration
builder.Configuration.AddJsonFile("appsettings.prod.json", optional: false, reloadOnChange: true);
#endregion Read Configuration

#region Serilog
builder.SerilogConfiguration();
#endregion Serilog

#region Layer Modules
builder.Services.AddDomainModule(builder.Configuration);
builder.Services.AddInfrastructurePersistenceModule(builder.Configuration);
builder.Services.AddInfrastructureAdapterModule(builder.Configuration);
builder.Services.AddApplicationModule(builder.Configuration);
builder.Services.AddInfrastructureHostModule();
#endregion Layer Modules

builder.Services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);

#region Api Rate Limit
builder.Services.ConfigureRateLimit();
#endregion Api Rate Limit

builder.Services.AddHangfire(configuration =>
{
    var hangfireConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    configuration.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(hangfireConnectionString));
});

builder.Services.AddHangfireServer();

builder.Services.AddHttpsRedirection(options => options.HttpsPort = 5001);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<LogHeadersMiddleware>();

app.UseSerilogRequestLogging();

app.UseMiddleware<ApiKeyMiddleware>();

//Remove comment to enable https app.UseHsts();

//Remove comment to enable https app.UseHttpsRedirection();

app.UseCors();

app.UseResponseCompression();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseRateLimiter();

app.UseMiddleware<SecureHeadersMiddleware>();

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    Authorization = new[] { new DashboardNoAuthorizationFilter() }
});

//Note: Swagger will not run before jobs finish
await HangfireStartupJob.EnqueueSynchronizeProvidersJob(app);

app.UseAuthentication(); //first line should be

app.UseAuthorization(); //second line should be

app.MapControllers();

app.Run();

/// <summary>
/// I need to declare Program for test containers nuget (integration testing)
/// </summary>
public partial class Program { }
