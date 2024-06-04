namespace TechnicalAnalysis.Infrastructure.Gateway
{
    public static class InfrastructureHostModule
    {
        public static void AddInfrastructureHostModule(this WebApplicationBuilder webApplicationBuilder, IConfiguration configuration)
        {
            var apiKeySection = configuration.GetSection("ApiKey");
            var baseAddressSection = configuration.GetSection("BaseAddress");

            // Configure HttpClient for all environments
            webApplicationBuilder.Services.AddHttpClient("taapi", client =>
            {
                client.BaseAddress = new Uri(baseAddressSection.Value ?? throw new ArgumentNullException($"{nameof(baseAddressSection.Value)} cannot be null"));
                client.DefaultRequestHeaders.Add("User-Agent", "Tracking prices application");
                client.DefaultRequestHeaders.Add("ApiKey", apiKeySection.Value);
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
