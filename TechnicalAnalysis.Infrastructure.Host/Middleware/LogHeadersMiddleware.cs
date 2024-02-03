namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class LogHeadersMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            foreach (var header in context.Request.Headers)
            {
                logger.LogInformation("Header: {Key}: {Value}", header.Key, header.Value);
            }

            await requestDelegate(context);
        }
    }
}
