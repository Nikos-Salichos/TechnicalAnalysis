using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

namespace TechnicalAnalysis.Infrastructure.Host.Services
{
    public static class CompressionExtension
    {
        public static IServiceCollection ConfigureCompression(this IServiceCollection services)
        {
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
