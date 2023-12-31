using Polly;

namespace TechnicalAnalysis.Domain.Interfaces.Utilities
{
    public interface IPollyPolicy
    {
        IAsyncPolicy<T> CreatePolicies<T>(int retries, TimeSpan timeSpan);
    }
}