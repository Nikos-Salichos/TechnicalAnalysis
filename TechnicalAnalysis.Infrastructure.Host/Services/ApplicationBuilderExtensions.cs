using TechnicalAnalysis.Infrastructure.Host.Middleware;

namespace TechnicalAnalysis.Infrastructure.Host.Services
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder AddCorrelationIdMiddleware(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
