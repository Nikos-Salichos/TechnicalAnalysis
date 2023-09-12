using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IAdapter
    {
        Task Sync(Provider provider, Timeframe timeframe = Timeframe.Daily);
    }
}