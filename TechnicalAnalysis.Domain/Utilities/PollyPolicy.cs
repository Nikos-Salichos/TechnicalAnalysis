using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Utilities
{
    public class PollyPolicy(ILogger<PollyPolicy> logger) : IPollyPolicy
    {
        private readonly ILogger<PollyPolicy> _logger = logger;

        public IAsyncPolicy<T> CreatePolicies<T>(int retries)
        {
            var retryDelays = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(5), retryCount: retries).ToArray();

            return Policy<T>.Handle<Exception>().WaitAndRetryAsync(
                retryDelays,
                onRetry: (exception, delay, retryAttempt, context) =>
                {
                    _logger.LogError("Retry attempt {RetryAttempt} of {Retries}. Delaying for {Delay} seconds, context {context}. " +
                        "Exception: {ExceptionResult} {ExceptionMessage} {ExceptionData}",
                        retryAttempt, retries, delay.TotalSeconds,
                        context, exception.Result, exception.Exception.Message, exception.Exception.Data);
                });
        }
    }
}
