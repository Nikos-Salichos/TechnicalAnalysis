using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TechnicalAnalysis.Infrastructure.Host.Filters
{
    public class ApiKeyHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            var apiKeyParameter = new OpenApiParameter
            {
                Name = "ApiKey",
                In = ParameterLocation.Header,
                Required = true,
                Example = new OpenApiString("123") // Example value
            };

            operation.Parameters.Add(apiKeyParameter);
        }

    }

}
