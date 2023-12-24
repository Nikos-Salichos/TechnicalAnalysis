using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Persistence.Repositories
{
    public class RedisRepository(IDistributedCache distributedCache) : IRedisRepository
    {
        private static readonly JsonSerializerOptions defaultJsonSerializerOptions = new()
        {
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public async Task SetRecordAsync<T>(string recordId,
                                          T data,
                                          TimeSpan? absoluteExpireTime = null,
                                          TimeSpan? slidingExpireTime = null)
        {
            var distributedCacheEntryOptions = GetDistributedCacheEntryOptions(absoluteExpireTime, slidingExpireTime);

            var jsonData = JsonSerializer.Serialize(data, defaultJsonSerializerOptions);
            await distributedCache.SetStringAsync(recordId, jsonData, distributedCacheEntryOptions, default);
        }

        public async Task<T?> GetRecordAsync<T>(string recordId)
        {
            var jsonData = await distributedCache.GetStringAsync(recordId);

            return jsonData is null
                ? default
                : JsonSerializer.Deserialize<T>(jsonData, defaultJsonSerializerOptions);
        }

        private static DistributedCacheEntryOptions GetDistributedCacheEntryOptions(TimeSpan? absoluteExpireTime,
            TimeSpan? slidingExpireTime)
        {
            DateTime endOfDay = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1); // Set time to 23:59:59.9999999

            return new()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? endOfDay - DateTime.UtcNow,
                SlidingExpiration = slidingExpireTime ?? endOfDay - DateTime.UtcNow
            };
        }
    }
}
