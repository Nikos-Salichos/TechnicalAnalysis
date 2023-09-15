using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
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

            var response = await client.GetAsync("api/v1/analysis/SynchronizeProviders");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
