using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtonPack.Data.Rules
{
    internal class StaticAllocation : BaseRule
    {
        public StaticAllocation(Guid behaviorTypeID) : base(behaviorTypeID)
        {
            RuleTypeID = new Guid("00000001-0000-0000-0000-000000000001");
        }

        protected override Rule Default(Rule rule)
        {
            rule.Parameters = new List<RuleParameter>
            {
                new RuleParameter
                {
                    ID = new Guid(),
                    ParameterName = "Default Parameter",
                    RuleID = rule.ID,
                    ParameterDataType = 0,
                    ParameterSequence = 0,
                    ParameterValue = "",
                }
            };
            return rule;
        }

        protected override decimal Process(Rule rule)
        {
            decimal rc = Convert.ToDecimal(rule.Parameters[0].ParameterValue);


            return rc;
        }

    }
}
