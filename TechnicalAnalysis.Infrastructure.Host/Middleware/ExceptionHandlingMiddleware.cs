using System.Net;
using System.Text.Json;

namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        public RequestDelegate requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.requestDelegate = requestDelegate;
            this.logger = logger;
        }

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

        private Task HandleException(HttpContext context, Exception exception)
        {
            logger.LogCritical("An exception occurred: {@ExceptionData}", exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            try
            {
                var json = JsonSerializer.Serialize(exception); // convert the object to a JSON string
                return context.Response.WriteAsync(json); // return the JSON string as the response
            }
            catch (Exception)
            {
                var errorDetail = new
                {
                    ExceptionMessage = exception.Message,
                    InnerExceptionMessage = exception?.InnerException?.Message,
                    StackTrace = exception?.StackTrace
                };

                return context.Response.WriteAsync(errorDetail?.ToString() ?? string.Empty);
            }
        }
    }
}
