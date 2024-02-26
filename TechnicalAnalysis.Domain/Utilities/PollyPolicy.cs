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
            var retryDelays = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(30), retryCount: retries).ToArray();

            return Policy.WrapAsync(
                Policy.TimeoutAsync<T>(timeSpan, TimeoutStrategy.Optimistic, onTimeoutAsync: (context, timespan, task) =>
                {
                    _logger.LogError("Timeout occurred after {timespan}. Context: {context}", timespan, context);
                    return Task.CompletedTask;
                }),
                Policy<T>.Handle<Exception>()
                    .WaitAndRetryAsync(retryDelays,
                        onRetry: (exception, delay, retryAttempt, _) =>
                        {
                            _logger.LogError("Retry attempt {retryAttempt} of {retries}. Delaying for {delay.TotalSeconds} seconds. Exception: {exception}",
                                retryAttempt, retries, delay.TotalSeconds, exception.Exception);
                        })
            );
        }

    }
}
