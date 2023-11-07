using FluentAssertions;
using Newtonsoft.Json;
using System.Text;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Tests.IntegrationTests.TestContainers.BaseClasses;

namespace TechnicalAnalysis.Tests.IntegrationTests.TestContainers
{
    public class ControllerTest : BaseIntegrationTest
    {
        private readonly IntegrationTestWebAppFactory _factory;
        public ControllerTest(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
        }

        //TODO Finish the test
        [Fact]
        public async Task Should_return_ok_on_http_get_synchronizeProviders()
        {
            var client = _factory.CreateClient();

            var dataProviderTimeframeRequest = new DataProviderTimeframeRequest(DataProvider.Binance, Timeframe.Weekly);

            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataProviderTimeframeRequest), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/v1/analysis/SynchronizeProviders", jsonContent);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
