using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Infrastructure.Persistence.DistributedCache
{
    public static class DistributedCacheExtension
    {
        private static readonly JsonSerializerOptions defaultJsonSerializerOptions = new()
        {
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
                                              string recordId,
                                              T data,
                                              TimeSpan? absoluteExpireTime = null,
                                              TimeSpan? slidingExpireTime = null)
        {
            var distributedCacheEntryOptions = GetDistributedCacheEntryOptions(absoluteExpireTime, slidingExpireTime);

            var jsonData = JsonSerializer.Serialize(data, defaultJsonSerializerOptions);
            await cache.SetStringAsync(recordId, jsonData, distributedCacheEntryOptions, default);
        }

        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

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
                SlidingExpiration = slidingExpireTime ?? TimeSpan.FromMinutes(1)
            };
        }

    }
}
