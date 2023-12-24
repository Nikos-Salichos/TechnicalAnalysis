using System.Net;
using System.Text.Json;

namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
    {
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
            logger.LogError("An exception occurred: {@Exception}", exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorDetail = new
            {
                ExceptionMessage = exception.Message,
                InnerExceptionMessage = exception.InnerException?.Message,
                exception.StackTrace
            };

            var json = JsonSerializer.Serialize(errorDetail);
            await context.Response.WriteAsync(json);
        }
    }
}
