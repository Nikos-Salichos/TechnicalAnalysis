using System.Net;
using System.Text.Json;

namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public RequestDelegate requestDelegate = requestDelegate;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (Exception exception)
            {
                await HandleException(context, exception);
            }
        }

        private async Task HandleException(HttpContext context, Exception exception)
        {
            logger.LogCritical("An exception occurred: {@Exception}", exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            try
            {
                var json = JsonSerializer.Serialize(exception);
                await context.Response.WriteAsync(json);
                return;
            }
            catch (Exception)
            {
                var errorDetail = new
                {
                    ExceptionMessage = exception.Message,
                    InnerExceptionMessage = exception?.InnerException?.Message,
                    StackTrace = exception?.StackTrace
                };

                await context.Response.WriteAsync(errorDetail?.ToString() ?? string.Empty);
                return;
            }
        }
    }
}
