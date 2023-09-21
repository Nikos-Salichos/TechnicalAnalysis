using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TechnicalAnalysis.Infrastructure.Persistence.DistributedCache
{
    public static class DistributedCacheExtension
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
                                              string recordId,
                                              T data,
                                              TimeSpan? absoluteExpireTime = null,
                                              TimeSpan? slidingExpireTime = null,
                                              HttpContext? httpContext = null)
        {
            DateTime now = DateTime.UtcNow;
            DateTime endOfDay = now.Date.AddDays(1).AddTicks(-1); // Set time to 23:59:59.9999999

            DistributedCacheEntryOptions distributedCacheEntryOptions = new()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? endOfDay - now,
                SlidingExpiration = slidingExpireTime
            };

            var jsonData = JsonSerializer.Serialize(data, jsonSerializerOptions);

            // Check if a specific header is present in the request
            if (httpContext != null && httpContext.Request.Headers.ContainsKey("C-Invalid"))
            {
                // If the specific header is present, invalidate the cache
                await cache.RemoveAsync(recordId);
            }
            else
            {
                // Otherwise, set the cache as usual
                await cache.SetStringAsync(recordId, jsonData, distributedCacheEntryOptions, default);
            }
        }


        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

            if (jsonData is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(jsonData, jsonSerializerOptions);
        }
    }
}
