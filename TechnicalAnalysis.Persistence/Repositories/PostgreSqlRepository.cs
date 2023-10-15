using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using TechnicalAnalysis.CommonModels.BusinessModels;
using TechnicalAnalysis.CommonModels.Enums;
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
    public class PostgreSqlRepository : IPostgreSqlRepository
    {
        private readonly string _connectionStringKey;
        private readonly ILogger<PostgreSqlRepository> _logger;

        public PostgreSqlRepository(IOptionsMonitor<DatabaseSetting> databaseSettings, ILogger<PostgreSqlRepository> logger)
        {
            _connectionStringKey = databaseSettings.CurrentValue.PostgreSqlTechnicalAnalysisDockerCompose;
            _logger = logger;
        }

        public async Task<IResult<IEnumerable<Asset>, string>> GetAssets()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                dbConnection.Open();
                const string query = "SELECT \"Id\" AS PrimaryId, \"Symbol\" AS Symbol FROM \"Assets\"";
                var assets = await dbConnection.QueryAsync<Asset>(query);
                dbConnection.Close();
                return Result<IEnumerable<Asset>, string>.Success(assets);
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(GetAssets), exception);
                return Result<IEnumerable<Asset>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<Candlestick>, string>> GetCandlesticks()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                dbConnection.Open();
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
                dbConnection.Close();
                return Result<IEnumerable<Candlestick>, string>.Success(candlesticks);
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(GetCandlesticks), exception);
                return Result<IEnumerable<Candlestick>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<Pair>, string>> GetPairs()
        {
            try
            {
                using NpgsqlConnection dbConnection = new NpgsqlConnection(_connectionStringKey);
                dbConnection.Open();
                const string query = "SELECT \"id\" AS PrimaryId, \"symbol\" AS Symbol, \"asset0_id\" AS BaseAssetId, \"asset1_id\" AS QuoteAssetId, \"provider_id\" AS Provider, \"is_active\" AS IsActive, \"all_candles\" AS AllCandles, \"created_at\" AS CreatedAt FROM \"Pairs\"";
                var pairs = await dbConnection.QueryAsync<Pair>(query);
                dbConnection.Close();
                return Result<IEnumerable<Pair>, string>.Success(pairs);
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(GetPairs), exception);
                return Result<IEnumerable<Pair>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<ProviderSynchronization>, string>> GetProviders()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                dbConnection.Open();

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

                var providers = dbConnection.Query<string>("select 'Binance' union select 'Uniswap' union select 'Pancakeswap' " +
                    "union select 'Alpaca' union select 'WallStreetZen' union select 'All'")
                    .Select(x => Enum.Parse(typeof(DataProvider), x)).ToList();

                var timeframes = dbConnection.Query<string>("select 'Daily' union select 'Weekly' union select 'OneHour'")
                    .Select(x => Enum.Parse(typeof(Timeframe), x)).ToList();

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

                dbConnection.Close();
                return Result<IEnumerable<ProviderSynchronization>, string>.Success(providerSynchronizations);
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(GetProviders), exception);
                return Result<IEnumerable<ProviderSynchronization>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<Pool>, string>> GetPools()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                dbConnection.Open();
                const string query = "SELECT \"Id\" AS PrimaryId, \"DexId\" AS Provider, \"PoolContract\", \"Token0Id\", \"Token0Contract\", \"Token1Id\", \"Token1Contract\", " +
                                     "\"FeeTier\" AS FeeTier, \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\", \"IsActive\" " +
                                     "FROM \"Pools\"";
                var pools = await dbConnection.QueryAsync<Pool>(query);
                dbConnection.Close();
                return Result<IEnumerable<Pool>, string>.Success(pools);
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(GetPools), exception);
                return Result<IEnumerable<Pool>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<DexCandlestick>, string>> GetDexCandlesticks()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                dbConnection.Open();
                const string query = "SELECT \"Id\" AS PrimaryId, \"PoolContract\", \"PoolId\", \"OpenDate\", \"Open\" AS OpenPrice, \"High\" AS HighPrice, \"Low\" AS LowPrice, " +
                                     "\"Close\" AS ClosePrice, \"Timeframe\", \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\" " +
                                     "FROM \"DexCandlesticks\"";
                var dexCandlesticks = await dbConnection.QueryAsync<DexCandlestick>(query);
                dbConnection.Close();
                return Result<IEnumerable<DexCandlestick>, string>.Success(dexCandlesticks);
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(GetDexCandlesticks), exception);
                return Result<IEnumerable<DexCandlestick>, string>.Fail(exception.ToString());
            }
        }

        public async Task InsertPairs(IEnumerable<Pair> pairs)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);

            const string query = "INSERT INTO \"Pairs\" (\"asset0_id\", \"asset1_id\", \"provider_id\", \"symbol\", \"is_active\", \"all_candles\", \"created_at\") " +
                        "VALUES (@BaseAssetId, @QuoteAssetId, @Provider, @Symbol, @IsActive, @AllCandles, @CreatedAt)";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, pairs, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(InsertPairs), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task InsertAssets(IEnumerable<Asset> assets)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "INSERT INTO \"Assets\" (\"Symbol\") VALUES (@Symbol)";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, assets, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(InsertAssets), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task InsertCandlesticks(IEnumerable<Candlestick> candlesticks)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "INSERT INTO \"Candlesticks\" (\"pair_id\", \"timeframe\", \"open_date\", \"close_date\", \"open_price\", \"high_price\", \"low_price\", \"close_price\", \"volume\", \"number_of_trades\") " +
                        "VALUES (@PairId, @Timeframe, @OpenDate, @CloseDate, @OpenPrice, @HighPrice, @LowPrice, @ClosePrice, @Volume, @NumberOfTrades)";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, candlesticks, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(InsertCandlesticks), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task InsertDexCandlesticks(IEnumerable<DexCandlestick> candlesticks)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "INSERT INTO \"DexCandlesticks\" (\"PoolContract\", \"PoolId\", \"OpenDate\", \"Open\", \"High\", \"Low\", \"Close\", \"Timeframe\", \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\")" +
                     "VALUES (@PoolContract, @PoolId, @OpenDate, @OpenPrice, @HighPrice, @LowPrice, @ClosePrice, @Timeframe, @Fees, @Liquidity, @TotalValueLocked, @Volume, @NumberOfTrades)";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, candlesticks, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(InsertDexCandlesticks), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task InsertPools(IEnumerable<Pool> pools)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "INSERT INTO \"Pools\" (\"DexId\", \"PoolContract\", \"Token0Id\", \"Token0Contract\", \"Token1Id\", \"Token1Contract\", \"FeeTier\", \"Fees\", \"Liquidity\", \"TotalValueLocked\", \"Volume\", \"TxCount\", \"IsActive\")" +
                                 "VALUES (@Provider, @PoolContract, @Token0Id, @Token0Contract, @Token1Id, @Token1Contract, @FeeTier, @Fees, @Liquidity, @TotalValueLocked, @Volume, @NumberOfTrades, @IsActive)";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, pools, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(InsertPools), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task UpdateProviderPairAssetSyncInfo(ProviderPairAssetSyncInfo providerPairAssetSyncInfos)
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
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, providerPairAssetSyncInfos, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(UpdateProviderPairAssetSyncInfo), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task UpdateProviderCandlestickSyncInfo(ProviderCandlestickSyncInfo candlestickSyncInfos)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "INSERT INTO public.\"ProviderCandlestickSyncInfos\" (\"ProviderId\", \"TimeframeId\", \"LastCandlestickSync\") " +
                                 "VALUES (@DataProvider, @Timeframe, @LastCandlestickSync) " +
                                 "ON CONFLICT (\"ProviderId\", \"TimeframeId\") DO UPDATE SET " +
                                 "\"LastCandlestickSync\" = EXCLUDED.\"LastCandlestickSync\"";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, candlestickSyncInfos, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(UpdateProviderCandlestickSyncInfo), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task DeleteDexCandlesticksByIds(IEnumerable<long> ids)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "DELETE FROM \"DexCandlesticks\" WHERE \"Id\" = ANY(@Ids)";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, new { Ids = ids }, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(DeleteDexCandlesticksByIds), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task DeletePoolsByIds(IEnumerable<long> ids)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "DELETE FROM \"Pools\" WHERE \"Id\" = ANY(@Ids)";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, new { Ids = ids }, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(DeletePoolsByIds), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task DeleteTokensByIds(IEnumerable<long> ids)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "DELETE FROM \"Tokens\" WHERE \"Id\" = ANY(@Ids)";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, new { Ids = ids }, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Method:{Method}, Exception{@exception}", nameof(DeleteTokensByIds), exception);
                transaction?.Rollback();
            }
            finally
            {
                transaction?.Dispose();
            }
        }
    }
}
