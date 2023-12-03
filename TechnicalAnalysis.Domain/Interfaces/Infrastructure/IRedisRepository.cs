namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IRedisRepository
    {
        Task SetRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);
        Task<T?> GetRecordAsync<T>(string recordId);
    }
}
