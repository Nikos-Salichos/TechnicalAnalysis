using FluentAssertions;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Tests.IntegrationTests.TestContainers.BaseClasses;

namespace TechnicalAnalysis.Tests.IntegrationTests.TestContainers
{
    public sealed class PostgreSqlRepositoryTest : BaseIntegrationTest
    {
        public PostgreSqlRepositoryTest(IntegrationTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task ExecuteAssetsCommand_Successful()
        {
            List<Asset> assets = new List<Asset>
            {
                new Asset { Symbol = "TestContainersAsset"},
            };

            await PostgreSqlRepository.InsertAssetsAsync(assets);

            var retrievedAssets = await PostgreSqlRepository.GetAssetsAsync();
            retrievedAssets.Should().NotBeNull();
            retrievedAssets.FailValue.Should().BeNull();
            retrievedAssets.SuccessValue.First().Symbol.Should().Be(assets[0].Symbol);
        }

        [Fact]
        public async Task InsertDuplicateAssets_Fail()
        {
            List<Asset> assets = new List<Asset>
            {
                new Asset { Symbol = "TestContainersAsset"},
                new Asset { Symbol = "TestContainersAsset"},
            };

            var insertedAssets = await PostgreSqlRepository.InsertAssetsAsync(assets);

            insertedAssets.FailValue.Should().NotBeEmpty();
            insertedAssets.SuccessValue.Should().BeNull();
        }

    }
}