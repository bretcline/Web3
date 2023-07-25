using ProtonPack.Data.Data.Partial;
using WebThree.Shared;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data
{

    public static class SystemSettingIDs
    {
        public static Guid MaxQueueCount { get => new("00000000-0000-0000-0000-000000000001"); }
        public static Guid ContractTypes { get => new("00000000-0000-0000-0000-000000000010"); }
    }


    public class SystemSettingDataManager : BaseDataManager<SystemSetting>
    {
        internal SystemSettingDataManager(CompanyUser cu) : base(cu)
        {
        }

        public ContractSettings GetContractSettings( Guid contractTypeId )
        {
            var rc = new ContractSettings();

            return rc;
        }
    }
}