using FluentAssertions;
using Microsoft.Extensions.Configuration;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Tests.IntegrationTests.TestContainers.BaseClasses;

namespace TechnicalAnalysis.Tests.IntegrationTests.TestContainers
{
    public sealed class ControllerTest(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task Should_Return_Unauthorized_On_HttpGet_SynchronizeProviders_Without_ApiKey()
        {
            var client = factory.CreateClient();

            string url = $"api/v1/analysis/SynchronizeProviders?dataProvider={DataProvider.Alpaca}&timeframe={Timeframe.Weekly}";

            var response = await client.GetAsync(url);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Should_Return_BadRequest_On_HttpGet_SynchronizeProviders()
        {
            var solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            var basePath = Path.Combine(solutionDirectory, "TechnicalAnalysis.Infrastructure.Host");

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json");

            var config = configBuilder.Build();

            var apiKey = config["ApiKey"];

            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("ApiKey", apiKey);

            string url = $"api/v1/analysis/SynchronizeProviders?dataProvider={DataProvider.Alpaca}&timeframe={Timeframe.Weekly}";

            var response = await client.GetAsync(url);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
