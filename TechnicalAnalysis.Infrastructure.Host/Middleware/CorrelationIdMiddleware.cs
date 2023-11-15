using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string _correlationIdHeader = "X-Correlation-Id";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = GetCorrelationId(context);
            AddCorrelationIdHeaderToResponse(context, correlationId);

            using (LogContext.PushProperty("correlation-id", correlationId))
            {
                await _next.Invoke(context);
            }
        }

        private static string GetCorrelationId(HttpContext context)
        {
            // Try to get the correlation ID from the request headers.
            if (context.Request.Headers.TryGetValue(_correlationIdHeader, out var correlationIdValue)
                && !StringValues.IsNullOrEmpty(correlationIdValue))
            {
                return correlationIdValue.ToString();
            }
            else
            {
                // If the header is not present or the value is empty, generate a new correlation ID.
                var newCorrelationId = Guid.NewGuid().ToString();
                context.Items[_correlationIdHeader] = newCorrelationId;
                return newCorrelationId;
            }
        }

        private static void AddCorrelationIdHeaderToResponse(HttpContext context, StringValues correlationId)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(_correlationIdHeader, new[] { correlationId.ToString() });
                return Task.CompletedTask;
            });
        }
    }

}
