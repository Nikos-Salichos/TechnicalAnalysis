using TechnicalAnalysis.CommonModels.BusinessModels;

namespace TechnicalAnalysis.Domain.Interfaces.Infrastructure
{
    public interface IWallStreetZenClient
    {
        IEnumerable<Stock> Sync();
    }
}
