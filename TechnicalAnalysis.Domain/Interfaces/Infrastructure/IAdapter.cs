using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IAdapter
    {
        Task Sync(DataProvider provider, Timeframe timeframe = Timeframe.Daily);
    }
}