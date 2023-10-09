namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class SecureHeadersMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate requestDelegate)
        {
            // Click jacking attack
            httpContext.Response.Headers.Add("X-Frame-Options", "DENY");

            // MIME-type sniffing attack
            httpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            //  site scripting attack
            httpContext.Response.Headers.Add("X-Xss-Protection", "1; mode=block");

            // Referring to un wanted site and reading the data
            httpContext.Response.Headers.Add("Referrer-Policy", "no-referrer");

            // Code Injection Attacks either click jacking or cross site scripting
            httpContext.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");
        }
    }
}
