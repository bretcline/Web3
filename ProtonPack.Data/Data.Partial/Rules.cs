using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtonPack.Data
{
    public partial class Rule 
    {
        [SugarColumn(IsIgnore = true)]
        public List<RuleParameter> Parameters { get; set; }

        [SugarColumn(IsIgnore = true)]
        public Group Group { get; set; }

        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(BehaviorTypeID))]
        public Attribute BehaviorType { get; set; }
        
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(RuleTypeID))]
        public Attribute RuleType { get; set; }
        
    }

}
