using Serilog;

namespace TechnicalAnalysis.Infrastructure.Host.Serilog
{
    public static class LogRequestEnricher
    {
        public static void LogAdditionalInfo(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
        }
    }
}
