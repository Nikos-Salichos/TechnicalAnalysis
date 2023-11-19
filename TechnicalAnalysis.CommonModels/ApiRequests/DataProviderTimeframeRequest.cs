using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.ApiRequests
{
    public readonly struct DataProviderTimeframeRequest(DataProvider dataProvider, Timeframe timeframe)
    {
        public DataProvider DataProvider { get; } = dataProvider;
        public Timeframe Timeframe { get; } = timeframe;
    }

}
