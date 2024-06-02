using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;
using Candlestick = TechnicalAnalysis.Domain.Entities.Candlestick;
using Pair = TechnicalAnalysis.Domain.Entities.Pair;
using PoolEntity = TechnicalAnalysis.Domain.Entities.PoolEntity;

namespace TechnicalAnalysis.Infrastructure.Persistence.Repositories
{
    public class PostgreSqlRepository(IOptionsMonitor<DatabaseSetting> databaseSettings, ILogger<PostgreSqlRepository> logger)
        : IPostgreSqlRepository
    {
        private readonly string _connectionStringKey = databaseSettings.CurrentValue.PostgreSqlTechnicalAnalysisDockerCompose;

        public async Task<IResult<List<FearAndGreedModel>, string>> GetCryptoFearAndGreedIndexAsync()
        {
            return await ExecutionTimeLogger.LogExecutionTime(async () =>
            {
                await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                const string query = "SELECT \"PrimaryId\", \"Value\", \"ValueClassificationType\", \"DateTime\" FROM \"CryptoFearAndGreedIndex\"";

                var assets = await dbConnection.QueryAsync<FearAndGreedModel>(query);
                return Result<List<FearAndGreedModel>, string>.Success(assets.ToList());
            }, logger);
        }

        public async Task<IResult<List<FearAndGreedModel>, string>> GetStockFearAndGreedIndexAsync()
        {
            return await ExecutionTimeLogger.LogExecutionTime(async () =>
            {
                await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                const string query = "SELECT \"PrimaryId\", \"Value\", \"ValueClassificationType\", \"DateTime\" FROM \"StockFearAndGreedIndex\"";

                var assets = await dbConnection.QueryAsync<FearAndGreedModel>(query);

                return Result<List<FearAndGreedModel>, string>.Success(assets.ToList());
            }, logger);
        }

        public async Task<IResult<List<Asset>, string>> GetAssetsAsync()
        {
            return await ExecutionTimeLogger.LogExecutionTime(async () =>
            {
                await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                const string query = "SELECT \"Id\" AS PrimaryId, \"Symbol\" AS Symbol, \"AssetType\" AS AssetType FROM \"Assets\"";
                var assets = await dbConnection.QueryAsync<Asset>(query);
                return Result<List<Asset>, string>.Success(assets.ToList());
            }, logger);
        }

        public async Task<IResult<List<AssetRanking>, string>> GetCoinPaprikaAssetsAsync()
        {
            return await ExecutionTimeLogger.LogExecutionTime(async () =>
            {
                await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                const string query = "SELECT \"Id\" AS Id," +
                "\"Name\" AS Name, " +
                "\"Symbol\" AS Symbol, " +
                "\"CreatedDate\" AS CreatedDate, " +
                "\"AssetType\" AS AssetType, " +
                "\"Provider\" AS DataProvider " +
                "FROM \"CoinPaprikaAssets\"";
                var assets = await dbConnection.QueryAsync<AssetRanking>(query);
                return Result<List<AssetRanking>, string>.Success(assets.ToList());
            }, logger);
        }

        public async Task<IResult<List<Candlestick>, string>> GetCandlesticksAsync()
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "SELECT \"Id\" AS PrimaryId, " +
                "\"open_date\" AS OpenDate, " +
                "\"open_price\" AS OpenPrice, " +
                "\"high_price\" AS HighPrice, " +
                "\"low_price\" AS LowPrice, " +
                "\"close_price\" AS ClosePrice," +
                " \"volume\" AS Volume, " +
                "\"close_date\" AS CloseDate, " +
                "\"number_of_trades\" AS NumberOfTrades," +
                " \"timeframe\" AS Timeframe, " +
                "\"pair_id\" AS PairId FROM \"Candlesticks\"";
            var candlesticks = await dbConnection.QueryAsync<Candlestick>(query);
            return Result<List<Candlestick>, string>.Success(candlesticks.ToList());
        }

        public async Task<IResult<List<Pair>, string>> GetPairsAsync()
        {
            await using NpgsqlConnection dbConnection = new(_connectionStringKey);
            const string query = "SELECT \"id\" AS PrimaryId, \"symbol\" AS Symbol, \"asset0_id\" AS BaseAssetId, \"asset1_id\" AS QuoteAssetId, \"provider_id\" AS Provider, \"is_active\" AS IsActive, \"all_candles\" AS AllCandles, \"created_at\" AS CreatedAt FROM \"Pairs\"";
            var pairs = await dbConnection.QueryAsync<Pair>(query);
            return Result<List<Pair>, string>.Success(pairs.ToList());
        }

        public async Task<IResult<List<ProviderSynchronization>, string>> GetProvidersAsync()
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);

            const string providerPairAssetSyncInfoQuery =
                                        """
                                            SELECT
                                            p."Id" AS PrimaryId,
                                            p."ProviderId" AS DataProvider,
                                            p."LastAssetSync",
                                            p."LastPairSync"
                                            FROM
                                            public."ProviderPairAssetSyncInfos" p
                                        """;

            const string providerCandlestickSyncInfoQuery =
                                               """
                                                SELECT
                                                p."Id" AS PrimaryId,
                                                p."ProviderId" AS DataProvider,
                                                p."LastCandlestickSync" AS LastCandlestickSync,
                                                p."TimeframeId" AS Timeframe
                                                FROM
                                                public."ProviderCandlestickSyncInfos" p 
                                               """;

            var syncInfos = await dbConnection.QueryMultipleAsync(providerPairAssetSyncInfoQuery + ";" + providerCandlestickSyncInfoQuery);

            var providerPairAssetSyncInfos = await syncInfos.ReadAsync<ProviderPairAssetSyncInfo>();
            var providerCandlestickSyncInfos = await syncInfos.ReadAsync<ProviderCandlestickSyncInfo>();

            var providerSynchronizations = providerPairAssetSyncInfos
                        .GroupJoin(providerCandlestickSyncInfos, pair => pair.DataProvider, candlestick => candlestick.DataProvider,
                            (pair, candlesticks) => new ProviderSynchronization()
                            {
                                ProviderPairAssetSyncInfo = pair,
                                CandlestickSyncInfos = candlesticks.ToList()
                            })
                        .ToList();

            return Result<List<ProviderSynchronization>, string>.Success(providerSynchronizations);
        }

        public async Task<IResult<List<PoolEntity>, string>> GetPoolsAsync()
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "SELECT \"Id\" AS PrimaryId, \"DexId\" AS Provider, \"PoolContract\", \"Token0Id\", \"Token0Contract\", \"Token1Id\", \"Token1Contract\", " +
                                 "\"FeeTier\" AS FeeTier, \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\", \"IsActive\" " +
                                 "FROM \"Pools\"";
            var pools = await dbConnection.QueryAsync<PoolEntity>(query);
            return Result<List<PoolEntity>, string>.Success(pools.ToList());
        }

        public async Task<IResult<List<DexCandlestick>, string>> GetDexCandlesticksAsync()
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "SELECT \"Id\" AS PrimaryId, \"PoolContract\", \"PoolId\", \"OpenDate\", \"Open\" AS OpenPrice, \"High\" AS HighPrice, \"Low\" AS LowPrice, " +
                                 "\"Close\" AS ClosePrice, \"Timeframe\", \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\" " +
                                 "FROM \"DexCandlesticks\"";
            var dexCandlesticks = await dbConnection.QueryAsync<DexCandlestick>(query);
            return Result<List<DexCandlestick>, string>.Success(dexCandlesticks.ToList());
        }

        public async Task InsertCryptoFearAndGreedIndex(List<FearAndGreedModel> fearAndGreedModels)
        {
            try
            {
                await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                await dbConnection.OpenAsync();

                await using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"CryptoFearAndGreedIndex\" (\"Value\", \"ValueClassificationType\", \"DateTime\") FROM STDIN BINARY");

                foreach (var index in fearAndGreedModels)
                {
                    await writer.StartRowAsync();
                    await WriteParameter(writer, index.Value);
                    await WriteParameter(writer, index.ValueClassificationType);
                    await WriteParameter(writer, index.DateTime);
                }

                await writer.CompleteAsync();
            }
            catch (Exception exception)
            {
                logger.LogError("Exception:{@exception}", exception);
            }
        }

        public async Task InsertStockFearAndGreedIndex(List<FearAndGreedModel> fearAndGreedModels)
        {
            try
            {
                const string query =
                               """
                                    INSERT INTO "StockFearAndGreedIndex" ("Value", "ValueClassificationType", "DateTime")
                                    VALUES (@Value, @ValueClassificationType, @DateTime)
                                    ON CONFLICT ("DateTime") DO UPDATE
                                    SET "Value" = EXCLUDED."Value",
                                        "ValueClassificationType" = EXCLUDED."ValueClassificationType"
                                """;

                await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                await dbConnection.OpenAsync();

                await using var transaction = await dbConnection.BeginTransactionAsync();
                await dbConnection.ExecuteAsync(query, fearAndGreedModels, transaction: transaction);
                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                logger.LogError("Exception:{@exception}", exception);
            }
        }

        public async Task InsertPairsAsync(List<Pair> pairs)
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            await dbConnection.OpenAsync();

            await using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"Pairs\" (\"asset0_id\", \"asset1_id\", \"provider_id\", \"symbol\", \"is_active\", \"all_candles\", \"created_at\") FROM STDIN BINARY");

            foreach (var pair in pairs)
            {
                await writer.StartRowAsync();
                await WriteParameter(writer, pair.BaseAssetId);
                await WriteParameter(writer, pair.QuoteAssetId);
                await WriteParameter(writer, (int)pair.Provider);
                await WriteParameter(writer, pair.Symbol);
                await WriteParameter(writer, pair.IsActive);
                await WriteParameter(writer, pair.AllCandles);
                await WriteParameter(writer, pair.CreatedAt);
            }

            await writer.CompleteAsync();
        }

        public async Task InsertAssetsAsync(List<Asset> assets)
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            await dbConnection.OpenAsync();
            await using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"Assets\" (\"Symbol\", \"CreatedDate\", \"AssetType\") FROM STDIN BINARY");

            foreach (var asset in assets)
            {
                await writer.StartRowAsync();
                await WriteParameter(writer, asset.Symbol);
                await WriteParameter(writer, asset.CreatedDate);
                await WriteParameter(writer, (long)asset.AssetType);
            }

            await writer.CompleteAsync();
        }

        public async Task InsertCoinPaprikaAssetsAsync(List<AssetRanking> assets)
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            await dbConnection.OpenAsync();
            await using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"CoinPaprikaAssets\" (\"Name\", \"Symbol\", \"CreatedDate\", \"AssetType\", \"Provider\") FROM STDIN BINARY");
            foreach (var asset in assets)
            {
                await writer.StartRowAsync();
                await WriteParameter(writer, asset.Name);
                await WriteParameter(writer, asset.Symbol);
                await WriteParameter(writer, asset.CreatedDate);
                await WriteParameter(writer, (long)asset.AssetType);
                await WriteParameter(writer, (long)asset.DataProvider);
            }

            await writer.CompleteAsync();
        }

        public async Task InsertCandlesticksAsync(List<Candlestick> candlesticks)
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            await dbConnection.OpenAsync();

            await using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"Candlesticks\" (\"pair_id\", \"timeframe\", \"open_date\", \"close_date\", \"open_price\", \"high_price\", \"low_price\", \"close_price\", \"volume\", \"number_of_trades\") FROM STDIN BINARY");
            foreach (var candlestick in candlesticks)
            {
                await writer.StartRowAsync();

                await WriteParameter(writer, candlestick.PairId);
                await WriteParameter(writer, (int)candlestick.Timeframe);
                await WriteParameter(writer, candlestick.OpenDate);
                await WriteParameter(writer, candlestick.CloseDate);
                await WriteParameter(writer, candlestick.OpenPrice);
                await WriteParameter(writer, candlestick.HighPrice);
                await WriteParameter(writer, candlestick.LowPrice);
                await WriteParameter(writer, candlestick.ClosePrice);
                await WriteParameter(writer, candlestick.Volume);
                await WriteParameter(writer, candlestick.NumberOfTrades);
            }

            await writer.CompleteAsync();
        }

        public async Task InsertDexCandlesticksAsync(List<DexCandlestick> candlesticks)
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            await dbConnection.OpenAsync();

            await using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"DexCandlesticks\" (\"PoolContract\", \"PoolId\", \"OpenDate\", \"Open\", \"High\", \"Low\", \"Close\", \"Timeframe\", \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\") FROM STDIN BINARY");

            foreach (var candlestick in candlesticks)
            {
                await writer.StartRowAsync();
                await WriteParameter(writer, candlestick.PoolContract);
                await WriteParameter(writer, candlestick.PoolId);
                await WriteParameter(writer, candlestick.OpenDate);
                await WriteParameter(writer, candlestick.OpenPrice);
                await WriteParameter(writer, candlestick.HighPrice);
                await WriteParameter(writer, candlestick.LowPrice);
                await WriteParameter(writer, candlestick.ClosePrice);
                await WriteParameter(writer, (int)candlestick.Timeframe);
                await WriteParameter(writer, candlestick.Fees);
                await WriteParameter(writer, candlestick.Liquidity);
                await WriteParameter(writer, candlestick.TotalValueLocked);
                await WriteParameter(writer, candlestick.Volume);
                await WriteParameter(writer, candlestick.NumberOfTrades);
            }

            await writer.CompleteAsync();
        }

        public async Task InsertPoolsAsync(List<PoolEntity> pools)
        {
            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            await dbConnection.OpenAsync();

            await using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"Pools\" (\"DexId\", \"PoolContract\", \"Token0Id\", \"Token0Contract\", \"Token1Id\", \"Token1Contract\", \"FeeTier\", \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\", \"IsActive\") FROM STDIN BINARY");

            foreach (var pool in pools)
            {
                await writer.StartRowAsync();
                await WriteParameter(writer, (long)pool.Provider);
                await WriteParameter(writer, pool.PoolContract);
                await WriteParameter(writer, pool.Token0Id);
                await WriteParameter(writer, pool.Token0Contract);
                await WriteParameter(writer, pool.Token1Id);
                await WriteParameter(writer, pool.Token1Contract);
                await WriteParameter(writer, pool.FeeTier);
                await WriteParameter(writer, pool.Fees);
                await WriteParameter(writer, pool.Liquidity);
                await WriteParameter(writer, pool.TotalValueLocked);
                await WriteParameter(writer, pool.Volume);
                await WriteParameter(writer, pool.NumberOfTrades);
                await WriteParameter(writer, pool.IsActive);
            }

            await writer.CompleteAsync();
        }

        public async Task UpdateProviderPairAssetSyncInfoAsync(ProviderPairAssetSyncInfo providerPairAssetSyncInfo)
        {
            const string query = "INSERT INTO \"ProviderPairAssetSyncInfos\" (\"ProviderId\", \"LastAssetSync\", \"LastPairSync\") " +
                                 "VALUES (@DataProvider, @LastAssetSync, @LastPairSync) " +
                                 "ON CONFLICT (\"ProviderId\") DO UPDATE SET " +
                                 "\"LastAssetSync\" = EXCLUDED.\"LastAssetSync\", " +
                                 "\"LastPairSync\" = EXCLUDED.\"LastPairSync\"";

            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            await dbConnection.OpenAsync();

            await using var transaction = await dbConnection.BeginTransactionAsync();
            await dbConnection.ExecuteAsync(query, providerPairAssetSyncInfo, transaction: transaction);
            await transaction.CommitAsync();
        }

        public async Task UpdateProviderCandlestickSyncInfoAsync(ProviderCandlestickSyncInfo providerCandlestickSyncInfo)
        {
            const string query = "INSERT INTO public.\"ProviderCandlestickSyncInfos\" (\"ProviderId\", \"TimeframeId\", \"LastCandlestickSync\") " +
                                 "VALUES (@DataProvider, @Timeframe, @LastCandlestickSync) " +
                                 "ON CONFLICT (\"ProviderId\", \"TimeframeId\") DO UPDATE SET " +
                                 "\"LastCandlestickSync\" = EXCLUDED.\"LastCandlestickSync\"";

            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            await dbConnection.OpenAsync();

            await using var transaction = await dbConnection.BeginTransactionAsync();
            await dbConnection.ExecuteAsync(query, providerCandlestickSyncInfo, transaction: transaction);
            await transaction.CommitAsync();
        }

        public async Task<IResult<string, string>> DeleteEntitiesByIdsAsync<T>(List<long> ids, string tableName)
        {
            var validTableNames = new HashSet<string> { "DexCandlesticks", "Pools" };
            if (!validTableNames.Contains(tableName))
            {
                return Result<string, string>.Fail("Invalid table");
            }

            string query = $"DELETE FROM \"{tableName}\" WHERE \"Id\" = ANY(@Ids)";

            await using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            await dbConnection.OpenAsync();

            await using var transaction = await dbConnection.BeginTransactionAsync();
            await dbConnection.ExecuteAsync(query, new { Ids = ids }, transaction: transaction);
            await transaction.CommitAsync();

            return Result<string, string>.Success(string.Empty);
        }

        private static async Task WriteParameter(NpgsqlBinaryImporter writer, object value)
        {
            if (value == null || value is DBNull)
            {
                await writer.WriteNullAsync();
            }
            else
            {
                switch (value)
                {
                    case ValueClassificationType enumValue:
                        // Convert enum to bigint
                        await writer.WriteAsync((int)enumValue, NpgsqlDbType.Bigint);
                        break;
                    default:
                        // Assuming other values are handled appropriately based on their types
                        await writer.WriteAsync(value);
                        break;
                }
            }
        }


    }
}
