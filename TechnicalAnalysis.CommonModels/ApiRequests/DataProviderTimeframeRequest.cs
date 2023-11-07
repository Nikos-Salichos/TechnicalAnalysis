using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.ApiRequests
{
    public struct DataProviderTimeframeRequest
    {
        public DataProvider DataProvider { get; }
        public Timeframe Timeframe { get; }

        public DataProviderTimeframeRequest(DataProvider dataProvider, Timeframe timeframe)
        {
            DataProvider = dataProvider;
            Timeframe = timeframe;
        }
    }

}
