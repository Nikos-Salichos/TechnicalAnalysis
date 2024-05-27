using Polly;

namespace TechnicalAnalysis.Domain.Interfaces.Utilities
{
    public interface IPollyPolicy
    {
        ResiliencePipeline CreatePolicies(int retries);
    }
}