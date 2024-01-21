using Hangfire;
using Hangfire.PostgreSql;
using TechnicalAnalysis.Application.Modules;
using TechnicalAnalysis.Infrastructure.Adapters.Modules;
using TechnicalAnalysis.Infrastructure.Host.Hangfire;
using TechnicalAnalysis.Infrastructure.Host.Middleware;
using TechnicalAnalysis.Infrastructure.Host.Modules;
using TechnicalAnalysis.Infrastructure.Host.Serilog;
using TechnicalAnalysis.Infrastructure.Host.Services;
using TechnicalAnalysis.Infrastructure.Persistence.Modules;

var builder = WebApplication.CreateBuilder(args);

#region Read Configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.prod.json", optional: false, reloadOnChange: true);
#endregion Read Configuration

#region Serilog
builder.SerilogConfiguration();
#endregion Serilog

#region Layer Modules
builder.Services.AddInfrastructurePersistenceModule(builder.Configuration);
builder.Services.AddInfrastructureAdapterModule(builder.Configuration);
builder.Services.AddApplicationModule(builder.Configuration);
builder.Services.AddInfrastructureHostModule();
#endregion Layer Modules

builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

#region Api Rate Limit
builder.Services.ConfigureRateLimit();
#endregion Api Rate Limit

builder.Services.AddHangfire(configuration =>
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    configuration.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)
     );
});

builder.Services.AddHangfireServer();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (builder.Environment.IsProduction())
{
    app.UseMiddleware<ApiKeyMiddleware>();
}

app.UseHsts();

app.UseHttpsRedirection();

app.UseCors();

app.UseResponseCompression();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseRateLimiter();

app.UseMiddleware<SecureHeadersMiddleware>();

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    Authorization = new[] { new DashboardNoAuthorizationFilter() }
});
await HangfireStartupJob.EnqueueSynchronizeProvidersJob(app);

app.UseAuthentication(); //first line should be

app.UseAuthorization(); //second line should be

app.MapControllers();

app.Run();

/// <summary>
/// I need to declare Program for test containers nuget (integration testing)
/// </summary>
public partial class Program { }
