using TechnicalAnalysis.CommonModels.BaseClasses;
using TechnicalAnalysis.CommonModels.Enums;

namespace TechnicalAnalysis.CommonModels.BusinessModels
{
    public class ProviderPairAssetSyncInfo : BaseEntity
    {
        public DataProvider DataProvider { get; init; }
        public DateTime LastAssetSync { get; private set; }
        public DateTime LastPairSync { get; private set; }

        public void UpdateProviderInfo()
        {
            LastAssetSync = DateTime.UtcNow;
            LastPairSync = DateTime.UtcNow;
        }
    }
}
