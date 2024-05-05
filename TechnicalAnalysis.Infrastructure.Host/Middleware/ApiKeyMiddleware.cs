using System.Net;

namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private const string APIKeyHeaderName = "ApiKey";
        private readonly string _hangfireDashboardPath = "/hangfire";

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(_hangfireDashboardPath))
            {
                // Allow unrestricted access to Hangfire dashboard
                await next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(APIKeyHeaderName, out var extractedApiKey))
            {
                await UnauthorizedResponse(context, "Api Key was not provided.");
                return;
            }

            var apiKey = configuration["ApiKey"];

            if (apiKey?.Equals(extractedApiKey, StringComparison.InvariantCultureIgnoreCase) != true)
            {
                await UnauthorizedResponse(context, "Unauthorized client.");
                return;
            }

            await next(context);
        }

        private static async Task UnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(message);
        }
    }
}
