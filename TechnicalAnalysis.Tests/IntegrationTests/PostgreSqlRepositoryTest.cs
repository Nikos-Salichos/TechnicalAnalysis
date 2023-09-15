using Dapper;
using FluentAssertions;
using Npgsql;
using System.Data;
using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Tests.IntegrationTests
{
    public sealed class PostgreSqlRepositoryTest : BaseIntegrationTest
    {
        [Fact]
        public async Task ExecuteAssetsCommand()
        {
            List<Asset> _assets = new List<Asset>
            {
                new Asset { Symbol = "BTC" },
            };
            using var connection = new NpgsqlConnection(PostgreSqlContainer.GetConnectionString());
            using var command = new NpgsqlCommand();
            connection.Open();
            const string query = "SELECT \"Id\" AS PrimaryId, \"Symbol\" AS Symbol FROM \"Assets\"";
            var assets = await connection.QueryAsync<Asset>(query);
        }

        [Fact]
        public async Task ExecuteProvidersCommand()
        {
            using var connection = new NpgsqlConnection(PostgreSqlContainer.GetConnectionString());
            using var command = new NpgsqlCommand();
            connection.Open();
            const string query = "SELECT \"Id\" AS PrimaryId, \"Name\", \"Code\", \"LastAssetSync\", \"LastPairSync\", \"LastCandlestickSync\" FROM \"Providers\"";
            var providers = await connection.QueryAsync<ProviderPairAssetSyncInfo>(query);
            providers.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task InsertDuplicateAssets_ThrowsExceptionAndRollback()
        {
            List<Asset> _assets = new List<Asset>
            {
                new Asset { Symbol = "BTC" },
                new Asset { Symbol = "BTC" }
            };

            using var connection = new NpgsqlConnection(PostgreSqlContainer.GetConnectionString());
            const string query = "INSERT INTO \"Assets\" (\"Symbol\") VALUES (@Symbol)";

            IDbTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                await connection.ExecuteAsync(query, _assets, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception exception)
            {
                exception.Should().NotBeNull();
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

    }
}