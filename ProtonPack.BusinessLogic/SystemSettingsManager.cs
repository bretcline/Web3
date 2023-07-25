using ProtonPack.Data;
using ProtonPack.Data.Data.Partial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebThree.Shared.Utilities;

namespace ProtonPack.BusinessLogic
{
    public class SystemSettingsManager : BaseBusinessLogicManager<SystemSetting>
    {
        public SystemSettingsManager(CompanyUser companyUser) : base(companyUser) { }

        public Task<ContractSettings?> GetContractSettings(Guid contractTypeId)
        {
            return Task.Run(() =>
            {
                return (manager as SystemSettingDataManager)?.GetContractSettings(contractTypeId) ?? null;
            });
        }
    }
}
