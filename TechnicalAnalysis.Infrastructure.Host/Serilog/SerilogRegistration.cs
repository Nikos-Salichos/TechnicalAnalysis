using Serilog;

namespace TechnicalAnalysis.Infrastructure.Host.Serilog
{
    public static class SerilogRegistration
    {
        public static void SerilogConfiguration(this WebApplicationBuilder builder)
        {
            var otelExporterOtlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

            if (string.IsNullOrWhiteSpace(otelExporterOtlpEndpoint))
            {
                throw new ArgumentNullException($"{nameof(otelExporterOtlpEndpoint)} cannot be string empty or null, please fill variable in docker-compose.yml");
            }

            builder.Host.UseSerilog((_, loggerConfiguration) => loggerConfiguration
                .WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = otelExporterOtlpEndpoint;
                })
                .ReadFrom.Configuration(builder.Configuration));

        }
    }
}