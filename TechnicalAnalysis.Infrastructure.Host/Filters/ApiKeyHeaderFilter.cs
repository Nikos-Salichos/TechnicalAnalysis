using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TechnicalAnalysis.Infrastructure.Host.Filters
{
    public class ApiKeyHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];

            var apiKeyParameter = new OpenApiParameter
            {
                Name = "ApiKey",
                In = ParameterLocation.Header,
                Required = true,
                Example = new OpenApiString("123")
            };

            var clearCacheParameter = new OpenApiParameter
            {
                Name = "X-Cache-Refresh",
                In = ParameterLocation.Header,
                Required = false,
                Example = new OpenApiString("true")
            };

            operation.Parameters.Add(apiKeyParameter);
            operation.Parameters.Add(clearCacheParameter);
        }
    }
}
