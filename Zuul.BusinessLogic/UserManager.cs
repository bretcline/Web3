using Zuul.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.BusinessLogic
{
    public class UserManager : BaseBusinessLogicManager<User>
    {
        public UserManager(CompanyUser companyUser) : base(companyUser) { }
    }
}
