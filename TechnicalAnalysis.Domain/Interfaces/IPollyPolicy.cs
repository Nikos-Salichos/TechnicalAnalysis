using Polly.Retry;
using Polly.Timeout;

namespace TechnicalAnalysis.Domain.Interfaces
{
    public interface IPollyPolicy
    {
        public AsyncRetryPolicy CreateRetryPolicy(int retries, TimeSpan retryInterval);

        public AsyncTimeoutPolicy CreateTimeoutPolicy(TimeSpan timeout);
    }
}
