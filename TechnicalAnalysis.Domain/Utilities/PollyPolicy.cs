using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;
using TechnicalAnalysis.Domain.Interfaces;

namespace TechnicalAnalysis.Domain.Utilities
{
    public class PollyPolicy : IPollyPolicy
    {
        private readonly ILogger<PollyPolicy> _logger;

        public PollyPolicy(ILogger<PollyPolicy> logger)
        {
            _logger = logger;
        }

        public IAsyncPolicy<T> CreatePolicies<T>(int retries, TimeSpan timeout)
        {
            Random random = new Random();

            return Policy.WrapAsync(
                    Policy.TimeoutAsync<T>(timeout, TimeoutStrategy.Optimistic, onTimeoutAsync: (context, timespan, task) =>
                    {
                        _logger.LogError("Timeout occurred after {timespan}. Context: {context}", timespan, context);
                        return Task.CompletedTask;
                    }),
               Policy<T>.Handle<Exception>()
                        .WaitAndRetryAsync(retries, retryAttempt =>
                        {
                            return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                        + TimeSpan.FromMilliseconds(random.Next(0, 1000));
                        },
                        onRetry: (exception, delay, retryAttempt, context) =>
                        {
                            _logger.LogError("Retry attempt {retryAttempt} of {retries}. Delaying for {delay.TotalSeconds} seconds. Exception: {exception}",
                                retryAttempt, retries, delay.TotalSeconds, exception.Exception);
                        })

            );
        }

    }
}
