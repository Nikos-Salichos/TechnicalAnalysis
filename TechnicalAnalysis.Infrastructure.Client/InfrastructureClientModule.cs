using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace TechnicalAnalysis.Infrastructure.Client
{
    public static class InfrastructureClientModule
    {
        public static void AddInfrastructurePersistenceModule(this IServiceCollection services)
        {
            services.AddSingleton<IAnalysisClient, AnalysisClient>();
            services.AddHttpClient("AnalysisClient", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://api/v1/analysis/");

                httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
                httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestsSample");
            });
        }
    }
}
