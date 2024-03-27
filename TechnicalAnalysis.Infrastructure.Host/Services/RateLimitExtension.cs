using System.Threading.RateLimiting;

namespace TechnicalAnalysis.Infrastructure.Host.Services
{
    public static class RateLimitExtension
    {
        public static IServiceCollection ConfigureRateLimit(this IServiceCollection services)
            => services.AddRateLimiter(options =>
                                   {
                                       options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                                       options.AddPolicy("fixed-by-ip", httpContext =>
                                           RateLimitPartition.GetFixedWindowLimiter(
                                               // Required if I run behind a reverse proxy, so I do not limit the proxy Ip Address
                                               // httpContext.Request.Headers["X-Forwarded-For"].ToString(), 
                                               partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                                               factory: _ => new FixedWindowRateLimiterOptions
                                               {
                                                   PermitLimit = 5,
                                                   Window = TimeSpan.FromMinutes(1)
                                               }));
                                   });
    }
}
