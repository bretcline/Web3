using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zuul.Data;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.BusinessLogic
{
    public abstract class BaseBusinessLogicManager<T> : RootBusinessLogicManager<T> where T : class
    {
        public BaseBusinessLogicManager(CompanyUser cu) : base(cu, DataManagerFactory.GetDataManager<T>)
        {
        }
    }
}
