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

if (builder.Environment.IsProduction())
{
    builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("PostgreSqlTechnicalAnalysisDockerCompose")));
    builder.Services.AddHangfireServer();
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHsts();

app.UseHttpsRedirection();

app.UseCors();

app.UseResponseCompression();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseRateLimiter();

app.UseMiddleware<SecureHeadersMiddleware>();

if (app.Environment.IsProduction())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions()
    {
        Authorization = new[] { new DashboardNoAuthorizationFilter() }
    });
    HangfireStartupJob.EnqueueSynchronizeProvidersJob(app);
}

app.UseAuthentication(); //first line should be

app.UseAuthorization(); //second line should be

app.MapControllers();

app.Run();

public partial class Program { }
