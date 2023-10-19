using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Tests.IntegrationTests
{
    public sealed class PostgreSqlRepositoryTest : BaseIntegrationTest, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IPostgreSqlRepository _postgreSqlRepository;

        public PostgreSqlRepositoryTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _postgreSqlRepository = (IPostgreSqlRepository)_factory.Services.GetService(typeof(IPostgreSqlRepository));
        }

        [Fact]
        public async Task ExecuteAssetsCommand_Successful()
        {
            await _postgreSqlRepository.DeleteAssetsAsync();

            List<Asset> assets = new List<Asset>
            {
                new Asset { Symbol = "TestContainersAsset"},
            };

            await _postgreSqlRepository.InsertAssetsAsync(assets);

            var retrievedAssets = await _postgreSqlRepository.GetAssetsAsync();
            retrievedAssets.Should().NotBeNull();
            retrievedAssets.FailValue.Should().BeNull();
            retrievedAssets.SuccessValue.First().Symbol.Should().Be(assets[0].Symbol);
        }

        [Fact]
        public async Task InsertDuplicateAssets_Fail()
        {
            await _postgreSqlRepository.DeleteAssetsAsync();

            List<Asset> assets = new List<Asset>
            {
                new Asset { Symbol = "TestContainersAsset"},
                new Asset { Symbol = "TestContainersAsset"},
            };

            await _postgreSqlRepository.InsertAssetsAsync(assets);

            var retrievedAssets = await _postgreSqlRepository.GetAssetsAsync();
            retrievedAssets.FailValue.Should().BeNull();
            retrievedAssets.SuccessValue.Should().BeEmpty();
        }

    }
}