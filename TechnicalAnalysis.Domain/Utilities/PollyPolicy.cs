using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
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

        public AsyncRetryPolicy CreateRetryPolicy(int retries, TimeSpan retryInterval)
        {
            return Policy.Handle<Exception>()
                         .WaitAndRetryAsync(retryCount: retries, sleepDurationProvider: _ => retryInterval, onRetry: (exception, timeSpan, retry, ctx) =>
                         {
                             _logger.LogWarning(exception,
                                 "Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                                 exception.GetType().Name,
                                 exception.Message,
                                 retry,
                                 retries);
                         });
        }

        public AsyncTimeoutPolicy CreateTimeoutPolicy(TimeSpan timespan)
        {
            return Policy.TimeoutAsync(timespan, TimeoutStrategy.Optimistic, onTimeoutAsync: (context, timespan, task) =>
            {
                _logger.LogWarning("Timeout occurred after {timespan}. Context: {context}", timespan, context);
                return Task.CompletedTask;
            });
        }
    }
}
