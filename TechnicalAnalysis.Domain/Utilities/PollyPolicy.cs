using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Reflection;
using TechnicalAnalysis.Domain.Interfaces.Utilities;

namespace TechnicalAnalysis.Domain.Utilities
{
    public class PollyPolicy(ILogger<PollyPolicy> logger) : IPollyPolicy
    {
        public ResiliencePipeline CreatePolicies(int retries)
        {
            var retryStrategyOptions = new RetryStrategyOptions
            {
                MaxRetryAttempts = retries,
                Delay = TimeSpan.FromSeconds(5),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                OnRetry = (args) =>
                {
                    // Use reflection to access the internal Options property
                    var optionsProperty = typeof(ResilienceProperties)
                        .GetProperty("Options", bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);

                    var options = optionsProperty?.GetValue(args.Context.Properties) as IDictionary<string, object>;

                    logger.LogError("Retry attempt {AttemptNumber} of {Retries}. Delaying for {RetryDelay} seconds. " +
                                    "Exception: {ExceptionMessage} {ExceptionData}, " +
                                    "Context: {Context}",
                                    args.AttemptNumber, retries, args.RetryDelay.TotalSeconds,
                                    args.Outcome.Exception?.Message, args.Outcome.Exception?.Data,
                                    options);
                    return default;
                }
            };

            return new ResiliencePipelineBuilder().AddRetry(retryStrategyOptions)
                                                  .AddTimeout(TimeSpan.FromMinutes(1)).Build();
        }
    }
}