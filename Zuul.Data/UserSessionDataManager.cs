using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.Data
{
    public class UserSessionDataManager : BaseDataManager<UserSession>
    {
        internal UserSessionDataManager(CompanyUser? cu) : base(cu)
        {
        }
    }
}