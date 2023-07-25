using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.Data
{
    public class CompanyDataManager : BaseDataManager<Company>
    {
        internal CompanyDataManager(CompanyUser? cu = null) : base(cu)
        {
        }
    }
}