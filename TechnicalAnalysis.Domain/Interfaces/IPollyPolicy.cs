using Polly;

namespace TechnicalAnalysis.Domain.Interfaces
{
    public interface IPollyPolicy
    {
        IAsyncPolicy<T> CreatePolicies<T>(int retries, TimeSpan timeSpan);
    }
}
