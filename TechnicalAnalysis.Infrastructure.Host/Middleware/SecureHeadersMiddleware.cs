namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class SecureHeadersMiddleware(RequestDelegate requestDelegate)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            AddHeaderIfNotExists(httpContext, "X-Frame-Options", "DENY");
            AddHeaderIfNotExists(httpContext, "X-Content-Type-Options", "nosniff");
            AddHeaderIfNotExists(httpContext, "X-Xss-Protection", "1; mode=block");
            AddHeaderIfNotExists(httpContext, "Referrer-Policy", "no-referrer");
            AddHeaderIfNotExists(httpContext, "Content-Security-Policy", "default-src 'self';");

            await requestDelegate(httpContext);
        }

        private static void AddHeaderIfNotExists(HttpContext context, string key, string value)
        {
            context.Response.Headers.TryAdd(key, value);
        }

    }
}
