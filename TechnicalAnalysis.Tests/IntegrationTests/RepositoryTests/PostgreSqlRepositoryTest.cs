﻿using FluentAssertions;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Tests.IntegrationTests.BaseClasses;

namespace TechnicalAnalysis.Tests.IntegrationTests.RepositoryTests
{
    public sealed class PostgreSqlRepositoryTest(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task ExecuteAssetsCommand_Successful()
        {
            List<Asset> assets =
            [
                new Asset { Symbol = "TestContainersAsset", ProductType = ProductType.Unknown},
            ];

            await PostgreSqlRepository.InsertAssetsAsync(assets);

            var retrievedAssets = await PostgreSqlRepository.GetAssetsAsync();
            retrievedAssets.Should().NotBeNull();
            retrievedAssets.FailValue.Should().BeNull();
            retrievedAssets.SuccessValue[0].Symbol.Should().Be(assets[0].Symbol);
        }

        [Fact]
        public async Task InsertAssetsAsync_ThrowsPostgresExceptionOnDuplicateKey()
        {
            List<Asset> assets =
            [
                new Asset { Symbol = "BTC", ProductType = ProductType.CryptoRegardlessLayer},
                new Asset { Symbol = "BTC", ProductType = ProductType.CryptoRegardlessLayer},
            ];

            var exception = await Assert.ThrowsAsync<Npgsql.PostgresException>(() => PostgreSqlRepository.InsertAssetsAsync(assets));

            Assert.Contains("23505: duplicate key value violates unique constraint", exception.Message);
        }
    }
}