using Serilog;

namespace TechnicalAnalysis.Infrastructure.Host.Serilog
{
    public static class SerilogRegistration
    {
        public static void SerilogConfiguration(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((_, loggerConfiguration) => loggerConfiguration
                               .ReadFrom.Configuration(builder.Configuration));
        }
    }
}