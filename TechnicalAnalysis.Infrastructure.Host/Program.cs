using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using TechnicalAnalysis.Application.Modules;
using TechnicalAnalysis.Infrastructure.Adapters.Modules;
using TechnicalAnalysis.Infrastructure.Host;
using TechnicalAnalysis.Infrastructure.Host.Hangfire;
using TechnicalAnalysis.Infrastructure.Host.Middleware;
using TechnicalAnalysis.Infrastructure.Host.Serilog;
using TechnicalAnalysis.Infrastructure.Host.Services;
using TechnicalAnalysis.Infrastructure.Persistence.Modules;

var builder = WebApplication.CreateBuilder(args);

#region Read Configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.prod.json", optional: true, reloadOnChange: true);
#endregion Read Configuration

#region Serilog
SerilogRegistration.SerilogConfiguration(builder);
#endregion Serilog

#region Cors
builder.Services.ConfigureCors();
#endregion Cors

#region Enums in Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums();
    options.SchemaFilter<EnumSchemaFilter>();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});
#endregion Enums in Swagger

#region Accept Json in Controller
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
#endregion Accept Json in Controller

#region Services Registration
builder.Services.AddInfrastructurePersistenceModule(builder.Configuration);
builder.Services.AddInfrastructureAdapterModule(builder.Configuration);
builder.Services.AddApplicationModule(builder.Configuration);

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("fixed-by-ip", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            // Required if I run behind a reverse proxy, so I do not limit the proxy Ip Address
            // httpContext.Request.Headers["X-Forwarded-For"].ToString(), 
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(1)
            }));
});

builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("PostgreSqlTechnicalAnalysisDockerCompose")));
builder.Services.AddHangfireServer();


#endregion Services Registration

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseRateLimiter(); // Always first in middleware order

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecureHeadersMiddleware>();
app.UseSerilogRequestLogging();

app.UseCors();

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    Authorization = new[] { new DashboardNoAuthorizationFilter() }
});

if (app.Environment.IsProduction())
{
    HangfireStartupJob.EnqueueSynchronizeProvidersJob(app);
}

// Authorization should come after Hangfire Dashboard.
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
