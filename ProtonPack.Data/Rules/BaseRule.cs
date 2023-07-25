using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data.Rules
{
    public interface IRuleDefinition
    {
        decimal ProcessRule(Rule rule, CompanyUser cu);
        Rule GetDefaultRule();
    }

    public abstract class BaseRule : IRuleDefinition
    {
        protected CompanyUser CompanyUser { get; set; }

        public Guid BehaviorTypeID { get; set; }    
        public Guid RuleTypeID { get; set; }

        public BaseRule( Guid behaviorTypeID )
        {
            BehaviorTypeID = behaviorTypeID;
        }

        protected abstract decimal Process( Rule rule );

        public decimal ProcessRule(Rule rule, CompanyUser cu)
        {
            CompanyUser = cu;
            if (rule != null)
            {
                return Process(rule);
            }
            else
                throw new WebThreeException("Null Rule");
        }

        public Rule GetDefaultRule()
        {
            Rule rule = new Rule
            {
                ID = new Guid(),
                RuleName = "Default Name",
                RuleDescription = "",
                BehaviorTypeID = BehaviorTypeID,
                RuleTypeID = RuleTypeID,
                GroupID = Guid.Empty
            };
            return Default( rule );
        }

        protected abstract Rule Default(Rule rule);
    }
}
