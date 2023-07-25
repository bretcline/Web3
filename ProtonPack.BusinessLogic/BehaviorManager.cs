using ProtonPack.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebThree.Shared.Utilities;

namespace ProtonPack.BusinessLogic
{
    public class BehaviorManager
    {
        protected CompanyUser companyUser { get; set; }

        public BehaviorManager(CompanyUser cu){ companyUser = cu; }

        public Task<decimal> ProcessLike(UserAssetData data)
        {
            return Task.Run(() =>
            {
                using var ruleManager = DataManagerFactory.GetDataManager<Rule, RuleDataManager>(companyUser);

                return ruleManager.ProcessLike(data);
            } );
        }

        public Task<decimal> ProcessComment(UserAssetData data)
        {
            return Task.Run(() =>
            {
                using var ruleManager = DataManagerFactory.GetDataManager<Rule, RuleDataManager>(companyUser);

                return ruleManager.ProcessComment(data);
            });
        }

        public Task<bool> ProcessTip(UserTipData data)
        {
            return Task.Run(() =>
            {
                using var ruleManager = DataManagerFactory.GetDataManager<Rule, RuleDataManager>(companyUser);

                return ruleManager.ProcessTip(data);
            });
        }
    }
}
