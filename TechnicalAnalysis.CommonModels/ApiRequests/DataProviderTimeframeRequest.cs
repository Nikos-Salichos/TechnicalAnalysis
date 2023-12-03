using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.ApiRequests
{
    public record DataProviderTimeframeRequest(DataProvider DataProvider, Timeframe Timeframe);
}
