using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data
{
    public class IpfsAttributeDataManager : BaseDataManager<IpfsAttribute>
    {

        internal IpfsAttributeDataManager(CompanyUser cu) : base(cu)
        {
        }

    }
}
