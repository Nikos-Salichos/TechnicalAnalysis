using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Infrastructure.Host.Modules
{
    public static class InfrastructureHostModule
    {
        public static IServiceCollection AddInfrastructureHostModule(this IServiceCollection services, IConfiguration configuration)
        {
            //Set Json options for controllers
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            //Accept Enums in Swagger
            services.AddSwaggerGen(options =>
            {
                options.UseInlineDefinitionsForEnums();
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            //Log Http Requests
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
                options.CombineLogs = true;
            });

            //Configure Cords
            services.AddCors(options => options.AddDefaultPolicy(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            //Brotli Compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            return services;
        }
    }
}
