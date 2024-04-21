using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Timeout;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Utilities
{
    public class PollyPolicy(ILogger<PollyPolicy> logger) : IPollyPolicy
    {
        private readonly ILogger<PollyPolicy> _logger = logger;

        public IAsyncPolicy<T> CreatePolicies<T>(int retries, TimeSpan timeSpan)
        {
            var retryDelays = DecorrelatedJitterBackoffV2(retries);

            var timeoutPolicy = Policy.TimeoutAsync<T>(
                timeSpan, TimeoutStrategy.Optimistic, onTimeoutAsync: (context, timespan, task) =>
                {
                    _logger.LogError("Timeout occurred after {TimeSpan}. Context: {Context}", timespan, context);
                    return Task.CompletedTask;
                });

            var retryPolicy = Policy<T>.Handle<Exception>().WaitAndRetryAsync(
                retryDelays,
                onRetry: (exception, delay, retryAttempt, context) =>
                {
                    _logger.LogError("Retry attempt {RetryAttempt} of {Retries}. Delaying for {Delay} seconds. Exception: {Exception}",
                        retryAttempt, retries, delay.TotalSeconds, exception);
                });

            return Policy.WrapAsync(timeoutPolicy, retryPolicy);
        }

        private static TimeSpan[] DecorrelatedJitterBackoffV2(int retries)
        {
            return Backoff.DecorrelatedJitterBackoffV2(
                medianFirstRetryDelay: TimeSpan.FromSeconds(5), retryCount: retries).ToArray();
        }
    }
}
