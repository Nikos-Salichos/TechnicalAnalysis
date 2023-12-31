using Microsoft.Extensions.Caching.Distributed;
using System.IO.Compression;
using System.Text.Json;
using TechnicalAnalysis.Domain.Helpers;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Infrastructure.Persistence.Repositories
{
    public class RedisRepository(IDistributedCache distributedCache) : IRedisRepository
    {
        public async Task SetRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var distributedCacheEntryOptions = GetDistributedCacheEntryOptions(absoluteExpireTime, slidingExpireTime);

            await using var compressedStream = new MemoryStream();
            await using (var brotliStream = new BrotliStream(compressedStream, CompressionMode.Compress))
                await JsonSerializer.SerializeAsync(brotliStream, data, JsonHelper.JsonSerializerOptions);

            await distributedCache.SetAsync(recordId, compressedStream.ToArray(), distributedCacheEntryOptions);
        }

        public async Task<T?> GetRecordAsync<T>(string recordId)
        {
            // Get the compressed data as a byte array
            var compressedData = await distributedCache.GetAsync(recordId);

            if (compressedData is null || compressedData.Length == 0)
            {
                return default;
            }

            // Decompress and deserialize the data in one step
            await using var compressedStream = new MemoryStream(compressedData);
            await using var brotliStream = new BrotliStream(compressedStream, CompressionMode.Decompress);
            return await JsonSerializer.DeserializeAsync<T>(brotliStream, JsonHelper.JsonSerializerOptions);
        }

        private static DistributedCacheEntryOptions GetDistributedCacheEntryOptions(TimeSpan? absoluteExpireTime,
            TimeSpan? slidingExpireTime)
        {
            var now = DateTime.UtcNow;
            var midnightUtc = now.Date.AddDays(1);
            var timeUntilMidnightUtc = midnightUtc - now;

            // Ensure there's always a non-zero duration
            timeUntilMidnightUtc = timeUntilMidnightUtc > TimeSpan.Zero
                ? timeUntilMidnightUtc
                : TimeSpan.FromSeconds(1);

            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? timeUntilMidnightUtc,
                SlidingExpiration = slidingExpireTime ?? timeUntilMidnightUtc
            };
        }


    }
}
