using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.ApiRequests
{
    public class DataProviderTimeframeRequest
    {
        public DataProvider DataProvider { get; init; } = DataProvider.All;
        public Timeframe Timeframe { get; init; } = Timeframe.Daily;
    }
}
