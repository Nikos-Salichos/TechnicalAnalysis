using System.Net;

namespace TechnicalAnalysis.Infrastructure.Host.Middleware
{
    public class ApiKeyMiddleware(RequestDelegate requestDelegate)
    {
        private const string APIKEYNAME = "ApiKey";

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                await UnauthorizedResponse(context, "Api Key was not provided. (Using ApiKeyMiddleware)");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEYNAME);

            if (apiKey?.Equals(extractedApiKey) != true)
            {
                await UnauthorizedResponse(context, "Unauthorized client. (Using ApiKeyMiddleware)");
                return;
            }

            await requestDelegate(context);
        }

        private static async Task UnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(message);
        }
    }
}
