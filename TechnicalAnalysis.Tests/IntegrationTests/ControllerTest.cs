using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Npgsql;
using System.Text;
using TechnicalAnalysis.CommonModels.ApiRequests;
using TechnicalAnalysis.CommonModels.Enums;
using Testcontainers.PostgreSql;

namespace TechnicalAnalysis.Tests.IntegrationTests
{
    public class ControllerTest : BaseIntegrationTest, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        //TODO Finish the test
        [Fact]
        public async Task Should_return_ok_on_http_get_synchronizeProviders()
        {
            using var connection = new NpgsqlConnection(PostgreSqlContainer.GetConnectionString());
            connection.Open();

            var client = _factory.CreateClient();

            var dataProviderTimeframeRequest = new DataProviderTimeframeRequest
            {
                DataProvider = DataProvider.Binance,
                Timeframe = Timeframe.Weekly
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataProviderTimeframeRequest), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/v1/analysis/SynchronizeProviders", jsonContent);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
