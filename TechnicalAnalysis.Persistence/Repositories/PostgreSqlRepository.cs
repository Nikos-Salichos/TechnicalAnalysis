using Dapper;
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
    public class PostgreSqlRepository : IPostgreSqlRepository
    {
        private readonly string _connectionStringKey;

        public PostgreSqlRepository(IOptionsMonitor<DatabaseSetting> databaseSettings)
        {
            _connectionStringKey = databaseSettings.CurrentValue.PostgreSqlTechnicalAnalysisDockerCompose;
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
                return Result<IEnumerable<Asset>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<Candlestick>, string>> GetCandlesticks()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                dbConnection.Open();
                const string query = "SELECT \"id\" AS PrimaryId, \"open_date\" AS OpenDate, \"open_price\" AS OpenPrice, \"high_price\" AS HighPrice, \"low_price\" AS LowPrice, \"close_price\" AS ClosePrice, \"volume\" AS Volume, \"close_date\" AS CloseDate, \"number_of_trades\" AS NumberOfTrades, \"timeframe\" AS Timeframe, \"pair_id\" AS PairId FROM \"Candlesticks\"";
                var candlesticks = await dbConnection.QueryAsync<Candlestick>(query);
                dbConnection.Close();
                return Result<IEnumerable<Candlestick>, string>.Success(candlesticks);
            }
            catch (Exception exception)
            {
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
                return Result<IEnumerable<Pair>, string>.Fail(exception.ToString());
            }
        }

        public async Task<IResult<IEnumerable<Exchange>, string>> GetExchanges()
        {
            try
            {
                using var dbConnection = new NpgsqlConnection(_connectionStringKey);
                dbConnection.Open();
                const string query = "SELECT \"Id\" AS PrimaryId, \"Name\", \"Code\", \"LastAssetSync\", \"LastPairSync\", \"LastCandlestickSync\" FROM \"Providers\"";
                var exchanges = await dbConnection.QueryAsync<Exchange>(query);
                dbConnection.Close();
                return Result<IEnumerable<Exchange>, string>.Success(exchanges);
            }
            catch (Exception exception)
            {
                return Result<IEnumerable<Exchange>, string>.Fail(exception.ToString());
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
            catch
            {
                transaction?.Rollback();
                throw;
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
            catch
            {
                transaction?.Rollback();
                throw;
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
            catch
            {
                transaction?.Rollback();
                throw;
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task InsertCandlesticks(IEnumerable<DexCandlestick> candlesticks)
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
            catch
            {
                transaction?.Rollback();
                throw;
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
            catch
            {
                transaction?.Rollback();
                throw;
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public async Task UpdateProvider(Exchange provider)
        {
            using var dbConnection = new NpgsqlConnection(_connectionStringKey);
            const string query = "UPDATE \"Providers\" " +
                       "SET \"Name\" = @Name, " +
                       "\"Code\" = @Code, " +
                       "\"LastAssetSync\" = @LastAssetSync, " +
                       "\"LastPairSync\" = @LastPairSync, " +
                       "\"LastCandlestickSync\" = @LastCandlestickSync " +
                       "WHERE \"Id\" = @PrimaryId";

            NpgsqlTransaction? transaction = null;
            try
            {
                dbConnection.Open();
                transaction = dbConnection.BeginTransaction();

                await dbConnection.ExecuteAsync(query, provider, transaction: transaction);

                transaction.Commit();
            }
            catch
            {
                transaction?.Rollback();
                throw;
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
            catch
            {
                transaction?.Rollback();
                throw;
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
            catch
            {
                transaction?.Rollback();
                throw;
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
            catch
            {
                transaction?.Rollback();
                throw;
            }
            finally
            {
                transaction?.Dispose();
            }
        }
    }
}
