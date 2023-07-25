using ProtonPack.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace ProtonPack.BusinessLogic
{

    public abstract class BaseBusinessLogicManager<T> : RootBusinessLogicManager<T> where T : class
    {
        public BaseBusinessLogicManager(CompanyUser cu) : base( cu, DataManagerFactory.GetDataManager<T> )
        {
        }
    }
}
