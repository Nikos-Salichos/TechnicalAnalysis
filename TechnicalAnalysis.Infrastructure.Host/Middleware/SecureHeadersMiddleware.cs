namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class SecureHeadersMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public SecureHeadersMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            AddHeaderIfNotExists(httpContext, "X-Frame-Options", "DENY");
            AddHeaderIfNotExists(httpContext, "X-Content-Type-Options", "nosniff");
            AddHeaderIfNotExists(httpContext, "X-Xss-Protection", "1; mode=block");
            AddHeaderIfNotExists(httpContext, "Referrer-Policy", "no-referrer");
            AddHeaderIfNotExists(httpContext, "Content-Security-Policy", "default-src 'self';");

            // Invoke the next middleware in the pipeline
            await _requestDelegate(httpContext);
        }

        private static void AddHeaderIfNotExists(HttpContext context, string key, string value)
        {
            context.Response.Headers.TryAdd(key, value);
        }

    }
}
