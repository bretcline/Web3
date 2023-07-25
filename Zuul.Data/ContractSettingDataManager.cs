using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.Data
{
    public class ContractSettingDataManager : BaseDataManager<ContractSetting>
    {
        internal ContractSettingDataManager(CompanyUser? cu = null) : base(cu)
        {
        }
    }

}