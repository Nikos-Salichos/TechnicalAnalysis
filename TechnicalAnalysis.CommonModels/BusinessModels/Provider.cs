using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class Provider : BaseEntity
    {
        public long ProviderId
        {
            get
            {
                return (long)ProviderName;
            }
        }

        public DataProvider ProviderName { get; init; }
        public DateTime LastAssetSync { get; set; }
        public DateTime LastPairSync { get; set; }
        public List<ProviderCandlestickSyncInfo> CandlestickSyncInfos { get; set; } = new List<ProviderCandlestickSyncInfo>();
    }
}
