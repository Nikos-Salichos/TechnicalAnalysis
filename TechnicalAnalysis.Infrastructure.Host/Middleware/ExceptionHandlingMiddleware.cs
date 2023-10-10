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
            var errorDetail = new
            {
                exception.Message,
                exception.InnerException,
                exception.StackTrace
            };

            logger.LogCritical(exception, "An exception occurred: {@ExceptionData}", errorDetail);

            var json = JsonSerializer.Serialize(errorDetail); // convert the object to a JSON string

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(json); // return the JSON string as the response
        }
    }
}
