namespace TechnicalAnalysis.Infrastructure.Gateway
{
    public static class InfrastructureHostModule
    {
        public static void AddInfrastructureHostModule(this WebApplicationBuilder webApplicationBuilder)
        {
            // Configure HttpClient for all environments
            webApplicationBuilder.Services.AddHttpClient("taapi", client =>
            {
                client.BaseAddress = new Uri("http://host.docker.internal:5000/api/v1/analysis/");
                client.DefaultRequestHeaders.Add("User-Agent", "Tracking prices application");
                client.DefaultRequestHeaders.Add("ApiKey", "tokalyteroapi");
            })
            // Add a delegating handler in the development environment which bypasses SSL validation
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                if (webApplicationBuilder.Environment.IsDevelopment())
                {
                    return new HttpClientHandler
                    {
                        // Only for development purposes and never for production
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                }
                else
                {
                    return new HttpClientHandler();
                }
            });
        }
    }
}
