using Serilog;

namespace TechnicalAnalysis.Infrastructure.Host.Serilog
{
    public static class SerilogRegistration
    {
        public static void SerilogConfiguration(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((_, loggerConfiguration) => loggerConfiguration
                               .ReadFrom.Configuration(builder.Configuration));
        }

        public static void SerilogConfiguration(HostBuilderContext hostContext, LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                 .ReadFrom.Configuration(hostContext.Configuration)
                 .Enrich.FromLogContext();
        }
    }
}