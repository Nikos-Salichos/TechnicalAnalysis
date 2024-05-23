using Serilog;

namespace TechnicalAnalysis.Infrastructure.Host.Serilog
{
    public static class SerilogRegistration
    {
        public static void SerilogConfiguration(this WebApplicationBuilder builder)
        {
            string? otelExporterOtlpEndpoint = null;

            if (builder.Environment.EnvironmentName != "IntegrationTest")
            {
                otelExporterOtlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

                if (string.IsNullOrWhiteSpace(otelExporterOtlpEndpoint))
                {
                    throw new ArgumentNullException(nameof(otelExporterOtlpEndpoint), "OTEL_EXPORTER_OTLP_ENDPOINT cannot be string empty or null, please fill variable in docker-compose.yml");
                }
            }

            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .WriteTo.OpenTelemetry(options =>
                    {
                        if (!string.IsNullOrWhiteSpace(otelExporterOtlpEndpoint))
                        {
                            options.Endpoint = otelExporterOtlpEndpoint;
                        }
                        // options.ResourceAttributes = new Dictionary<string, object>
                        // {
                        //     { "service.name", "TechnicalAnalysis" }
                        // };
                    })
                    .ReadFrom.Configuration(context.Configuration);
            });

        }
    }
}