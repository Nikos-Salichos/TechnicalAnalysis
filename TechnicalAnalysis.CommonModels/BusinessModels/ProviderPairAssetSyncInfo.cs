using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class ProviderPairAssetSyncInfo : BaseEntity
    {
        public DataProvider DataProvider { get; set; }
        public DateTime LastAssetSync { get; set; }
        public DateTime LastPairSync { get; set; }

        public ProviderPairAssetSyncInfo(DataProvider dataProvider)
        {
            DataProvider = dataProvider;
        }
    }
}
