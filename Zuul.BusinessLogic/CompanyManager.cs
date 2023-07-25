using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zuul.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.BusinessLogic
{
    public class CompanyManager : BaseBusinessLogicManager<Company>
    {
        public CompanyManager(CompanyUser companyUser) : base(companyUser) { }
    }

    public class ContractSettingManager : BaseBusinessLogicManager<ContractSetting>
    {
        public ContractSettingManager(CompanyUser companyUser) : base(companyUser) { }
    }


}
