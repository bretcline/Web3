using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.Data
{
    public class ValidationDataManager : BaseDataManager<Validation>
    {
        internal ValidationDataManager(CompanyUser? cu) : base(cu)
        {
        }

        public Validation GetUnvalidated(Guid id)
        {
            return First(i => i.ID == id && !i.Validated );
        }
    }
}