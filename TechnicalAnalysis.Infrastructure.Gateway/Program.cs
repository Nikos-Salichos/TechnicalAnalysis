using TechnicalAnalysis.Infrastructure.Gateway;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddInfrastructureHostModule();

var proxyBuilder = builder.Services.AddReverseProxy();
// Initialize the reverse proxy from the "ReverseProxy" section of configuration
proxyBuilder.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//builder.Services.AddLettuceEncrypt();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseHsts();

app.UseHttpsRedirection();

app.MapReverseProxy();

app.UseAuthorization();

app.MapControllers();

app.Run();
