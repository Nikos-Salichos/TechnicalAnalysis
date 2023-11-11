using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.Domain.Entities;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;
using TechnicalAnalysis.Domain.Interfaces.Utilities;
using TechnicalAnalysis.Domain.Settings;
using TechnicalAnalysis.Domain.Utilities;
using Candlestick = TechnicalAnalysis.Domain.Entities.Candlestick;
using Pair = TechnicalAnalysis.Domain.Entities.Pair;
using Pool = TechnicalAnalysis.Domain.Entities.Pool;

namespace TechnicalAnalysis.Infrastructure.Persistence.Repositories
{
    //TODO Change methods to async
    //TODO Remove open & close connection (queryAsync will handle it)
    public class PostgreSqlRepository : IPostgreSqlRepository
    {
        private readonly string _connectionStringKey;
        private readonly ILogger<PostgreSqlRepository> _logger;

        public PostgreSqlRepository(IOptionsMonitor<DatabaseSetting> databaseSettings, ILogger<PostgreSqlRepository> logger)
        {
            _connectionStringKey = databaseSettings.CurrentValue.PostgreSqlTechnicalAnalysisDockerCompose;
            _logger = logger;
        }

        public async Task<IResult<IEnumerable<Asset>, string>> GetAssetsAsync()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                const string query = "SELECT \"Id\" AS PrimaryId, \"Symbol\" AS Symbol FROM \"Assets\"";
                var assets = await dbConnection.QueryAsync<Asset>(query);
                return Result<IEnumerable<Asset>, string>.Success(assets);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(GetAssetsAsync), exception);
                return Result<IEnumerable<Asset>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<Candlestick>, string>> GetCandlesticksAsync()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
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
                return Result<IEnumerable<Candlestick>, string>.Success(candlesticks);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(GetCandlesticksAsync), exception);
                return Result<IEnumerable<Candlestick>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<Pair>, string>> GetPairsAsync()
        {
            try
            {
                using NpgsqlConnection dbConnection = new NpgsqlConnection(_connectionStringKey);
                const string query = "SELECT \"id\" AS PrimaryId, \"symbol\" AS Symbol, \"asset0_id\" AS BaseAssetId, \"asset1_id\" AS QuoteAssetId, \"provider_id\" AS Provider, \"is_active\" AS IsActive, \"all_candles\" AS AllCandles, \"created_at\" AS CreatedAt FROM \"Pairs\"";
                var pairs = await dbConnection.QueryAsync<Pair>(query);
                return Result<IEnumerable<Pair>, string>.Success(pairs);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(GetPairsAsync), exception);
                return Result<IEnumerable<Pair>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<ProviderSynchronization>, string>> GetProvidersAsync()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);

                const string providerPairAssetSyncInfoQuery = @"
                                        SELECT
                                            p.""Id"" AS PrimaryId,
                                            p.""ProviderId"" AS DataProvider,
                                            p.""LastAssetSync"",
                                            p.""LastPairSync""
                                        FROM
                                            public.""ProviderPairAssetSyncInfos"" p";

                const string providerCandlestickSyncInfoQuery = @"
                                            SELECT
                                                p.""Id"" AS PrimaryId,
                                                p.""ProviderId"" AS DataProvider,
                                                p.""LastCandlestickSync"" AS LastCandlestickSync,
                                                p.""TimeframeId"" AS Timeframe
                                            FROM
                                                public.""ProviderCandlestickSyncInfos"" p";

                var providerPairAssetSyncInfos = await dbConnection.QueryAsync<ProviderPairAssetSyncInfo>(providerPairAssetSyncInfoQuery);
                var providerCandlestickSyncInfos = await dbConnection.QueryAsync<ProviderCandlestickSyncInfo>(providerCandlestickSyncInfoQuery);

                var providerSynchronizations = new List<ProviderSynchronization>();
                foreach (var providerPairAssetSyncInfo in providerPairAssetSyncInfos)
                {
                    var providerSynchronization = new ProviderSynchronization(providerPairAssetSyncInfo.DataProvider);
                    providerSynchronization.ProviderPairAssetSyncInfo = providerPairAssetSyncInfo;
                    providerSynchronizations.Add(providerSynchronization);
                }

                foreach (var providerCandlestickSyncInfo in providerCandlestickSyncInfos)
                {
                    var providerSynchronizationFound = providerSynchronizations.Find(p => p.DataProvider == providerCandlestickSyncInfo.DataProvider);
                    providerSynchronizationFound?.CandlestickSyncInfos.Add(providerCandlestickSyncInfo);
                }

                return Result<IEnumerable<ProviderSynchronization>, string>.Success(providerSynchronizations);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(GetProvidersAsync), exception);
                return Result<IEnumerable<ProviderSynchronization>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<Pool>, string>> GetPoolsAsync()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                const string query = "SELECT \"Id\" AS PrimaryId, \"DexId\" AS Provider, \"PoolContract\", \"Token0Id\", \"Token0Contract\", \"Token1Id\", \"Token1Contract\", " +
                                     "\"FeeTier\" AS FeeTier, \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\", \"IsActive\" " +
                                     "FROM \"Pools\"";
                var pools = await dbConnection.QueryAsync<Pool>(query);
                return Result<IEnumerable<Pool>, string>.Success(pools);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(GetPoolsAsync), exception);
                return Result<IEnumerable<Pool>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<DexCandlestick>, string>> GetDexCandlestickssAsync()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                const string query = "SELECT \"Id\" AS PrimaryId, \"PoolContract\", \"PoolId\", \"OpenDate\", \"Open\" AS OpenPrice, \"High\" AS HighPrice, \"Low\" AS LowPrice, " +
                                     "\"Close\" AS ClosePrice, \"Timeframe\", \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\" " +
                                     "FROM \"DexCandlesticks\"";
                var dexCandlesticks = await dbConnection.QueryAsync<DexCandlestick>(query);
                return Result<IEnumerable<DexCandlestick>, string>.Success(dexCandlesticks);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(GetDexCandlestickssAsync), exception);
                return Result<IEnumerable<DexCandlestick>, string>.Fail(exception.ToString());
            }
        }

        public async Task InsertPairsAsync(IEnumerable<Pair> pairs)
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                await dbConnection.OpenAsync();

                using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"Pairs\" (\"asset0_id\", \"asset1_id\", \"provider_id\", \"symbol\", \"is_active\", \"all_candles\", \"created_at\") FROM STDIN BINARY");

                foreach (var pair in pairs)
                {
                    await writer.StartRowAsync();
                    await WriteParameter(writer, pair.BaseAssetId);
                    await WriteParameter(writer, pair.QuoteAssetId);
                    await WriteParameter(writer, pair.Provider);
                    await WriteParameter(writer, pair.Symbol);
                    await WriteParameter(writer, pair.IsActive);
                    await WriteParameter(writer, pair.AllCandles);
                    await WriteParameter(writer, pair.CreatedAt);
                }

                await writer.CompleteAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(InsertPairsAsync), exception);
            }
        }

        public async Task<IResult<string, string>> InsertAssetsAsync(IEnumerable<Asset> assets)
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                await dbConnection.OpenAsync();
                using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"Assets\" (\"Symbol\", \"CreatedDate\") FROM STDIN BINARY");

                foreach (var asset in assets)
                {
                    await writer.StartRowAsync();
                    await WriteParameter(writer, asset.Symbol);
                    await WriteParameter(writer, asset.CreatedDate);
                }

                await writer.CompleteAsync();
                return Result<string, string>.Success(string.Empty);
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(InsertAssetsAsync), exception);
                return Result<string, string>.Fail(exception.ToString());
            }
        }

        public async Task InsertCandlesticksAsync(IEnumerable<Candlestick> candlesticks)
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                await dbConnection.OpenAsync();

                using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"Candlesticks\" (\"pair_id\", \"timeframe\", \"open_date\", \"close_date\", \"open_price\", \"high_price\", \"low_price\", \"close_price\", \"volume\", \"number_of_trades\") FROM STDIN BINARY");
                foreach (var candlestick in candlesticks)
                {
                    await writer.StartRowAsync();

                    await WriteParameter(writer, candlestick.PairId);
                    await WriteParameter(writer, (int)candlestick.Timeframe);
                    await WriteParameter(writer, candlestick.OpenDate);
                    await WriteParameter(writer, candlestick.CloseDate);
                    await WriteParameter(writer, candlestick?.OpenPrice);
                    await WriteParameter(writer, candlestick?.HighPrice);
                    await WriteParameter(writer, candlestick?.LowPrice);
                    await WriteParameter(writer, candlestick?.ClosePrice);
                    await WriteParameter(writer, candlestick?.Volume);
                    await WriteParameter(writer, candlestick?.NumberOfTrades);
                }

                await writer.CompleteAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(InsertCandlesticksAsync), exception);
            }
        }

        public async Task InsertDexCandlesticksAsync(IEnumerable<DexCandlestick> candlesticks)
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                await dbConnection.OpenAsync();

                using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"DexCandlesticks\" (\"PoolContract\", \"PoolId\", \"OpenDate\", \"Open\", \"High\", \"Low\", \"Close\", \"Timeframe\", \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\") FROM STDIN BINARY");

                foreach (var candlestick in candlesticks)
                {
                    await writer.StartRowAsync();
                    await WriteParameter(writer, candlestick.PoolContract);
                    await WriteParameter(writer, candlestick.PoolId);
                    await WriteParameter(writer, candlestick.OpenDate);
                    await WriteParameter(writer, candlestick?.OpenPrice);
                    await WriteParameter(writer, candlestick?.HighPrice);
                    await WriteParameter(writer, candlestick?.LowPrice);
                    await WriteParameter(writer, candlestick?.ClosePrice);
                    await WriteParameter(writer, (int)candlestick?.Timeframe);
                    await WriteParameter(writer, candlestick?.Fees);
                    await WriteParameter(writer, candlestick?.Liquidity);
                    await WriteParameter(writer, candlestick?.TotalValueLocked);
                    await WriteParameter(writer, candlestick?.Volume);
                    await WriteParameter(writer, candlestick?.NumberOfTrades);
                }

                await writer.CompleteAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(InsertDexCandlesticksAsync), exception);
            }
        }

        public async Task InsertPoolsAsync(IEnumerable<Pool> pools)
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                await dbConnection.OpenAsync();

                using var writer = await dbConnection.BeginBinaryImportAsync("COPY \"Pools\" (\"DexId\", \"PoolContract\", \"Token0Id\", \"Token0Contract\", \"Token1Id\", \"Token1Contract\", \"FeeTier\", \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\", \"IsActive\") FROM STDIN BINARY");

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
                    await WriteParameter(writer, pool?.Fees);
                    await WriteParameter(writer, pool?.Liquidity);
                    await WriteParameter(writer, pool?.TotalValueLocked);
                    await WriteParameter(writer, pool?.Volume);
                    await WriteParameter(writer, pool?.NumberOfTrades);
                    await WriteParameter(writer, pool?.IsActive);
                }

                await writer.CompleteAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(InsertPoolsAsync), exception);
            }
        }

        public async Task UpdateProviderPairAssetSyncInfoAsync(ProviderPairAssetSyncInfo providerPairAssetSyncInfos)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);

            const string query = "INSERT INTO \"ProviderPairAssetSyncInfos\" (\"ProviderId\", \"LastAssetSync\", \"LastPairSync\") " +
                                 "VALUES (@DataProvider, @LastAssetSync, @LastPairSync) " +
                                 "ON CONFLICT (\"ProviderId\") DO UPDATE SET " +
                                 "\"LastAssetSync\" = EXCLUDED.\"LastAssetSync\", " +
                                 "\"LastPairSync\" = EXCLUDED.\"LastPairSync\"";

            NpgsqlTransaction? transaction = null;
            try
            {
                await dbConnection.OpenAsync();
                transaction = await dbConnection.BeginTransactionAsync();

                await dbConnection.ExecuteAsync(query, providerPairAssetSyncInfos, transaction: transaction);

                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(UpdateProviderPairAssetSyncInfoAsync), exception);
                await transaction?.RollbackAsync();
            }
            finally
            {
                await dbConnection.CloseAsync();
                transaction?.DisposeAsync();
            }
        }

        public async Task UpdateProviderCandlestickSyncInfoAsync(ProviderCandlestickSyncInfo candlestickSyncInfos)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "INSERT INTO public.\"ProviderCandlestickSyncInfos\" (\"ProviderId\", \"TimeframeId\", \"LastCandlestickSync\") " +
                                 "VALUES (@DataProvider, @Timeframe, @LastCandlestickSync) " +
                                 "ON CONFLICT (\"ProviderId\", \"TimeframeId\") DO UPDATE SET " +
                                 "\"LastCandlestickSync\" = EXCLUDED.\"LastCandlestickSync\"";

            NpgsqlTransaction? transaction = null;
            try
            {
                await dbConnection.OpenAsync();
                transaction = await dbConnection.BeginTransactionAsync();

                await dbConnection.ExecuteAsync(query, candlestickSyncInfos, transaction: transaction);

                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", nameof(UpdateProviderCandlestickSyncInfoAsync), exception);
                await transaction?.RollbackAsync();
            }
            finally
            {
                await dbConnection.CloseAsync();
                transaction?.DisposeAsync();
            }
        }

        public async Task DeleteEntitiesByIdsAsync<T>(IEnumerable<long> ids, string tableName, string idColumnName)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            string query = $"DELETE FROM \"{tableName}\" WHERE \"{idColumnName}\" = ANY(@Ids)";

            NpgsqlTransaction? transaction = null;
            try
            {
                await dbConnection.OpenAsync();
                transaction = await dbConnection.BeginTransactionAsync();

                await dbConnection.ExecuteAsync(query, new { Ids = ids }, transaction: transaction);

                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Method:{Method}, Exception{@exception}", $"Delete{typeof(T).Name}ByIdsAsync", exception);
                await transaction?.RollbackAsync();
            }
            finally
            {
                await dbConnection.CloseAsync();
                transaction?.Dispose();
            }
        }

        private static async Task WriteParameter(NpgsqlBinaryImporter writer, object value)
        {
            if (value == null || value is DBNull)
            {
                await writer.WriteNullAsync();
            }
            else
            {
                await writer.WriteAsync(value);
            }
        }

    }
}
