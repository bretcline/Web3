using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtonPack.Data.Rules
{
    internal class RandomAllocation : BaseRule
    {
        public RandomAllocation( Guid behaviorTypeID ) : base( behaviorTypeID ) 
        {
            RuleTypeID = new Guid("00000001-0000-0000-0000-000000000002");
        }

        protected override Rule Default(Rule rule)
        {
            var paramList = new List<RuleParameter>();
            paramList.Add(new RuleParameter
            {
                ID = new Guid(),
                ParameterName = "Min Value Parameter",
                RuleID = rule.ID,
                ParameterDataType = 0,
                ParameterSequence = 0,
                ParameterValue = "",
            });
            paramList.Add(new RuleParameter
            {
                ID = new Guid(),
                ParameterName = "Max Value Parameter",
                RuleID = rule.ID,
                ParameterDataType = 0,
                ParameterSequence = 0,
                ParameterValue = "",
            });
            rule.Parameters = paramList;
            return rule;
        }

        protected override decimal Process(Rule rule)
        {
            var min = Convert.ToInt32(rule.Parameters[0].ParameterValue) * 1000;
            var max = Convert.ToInt32(rule.Parameters[1].ParameterValue) * 1000;

            return Random.Shared.Next(min, max) / 1000M; ;
        }
    }
}
