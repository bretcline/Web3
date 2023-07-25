using ProtonPack.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebThree.Shared.Utilities;


namespace ProtonPack.BusinessLogic
{
    public class ClaimManager : BaseBusinessLogicManager<Claim>
    {
        public ClaimManager(CompanyUser companyUser) : base(companyUser) { }

        public List<Claim> GetAllByParentAssetID(Guid parentAssetId)
        {
            return (manager as ClaimDataManager)?.GetByParentAsset(parentAssetId) ?? new List<Claim>();
        }
        public List<Claim> GetAllByClaimTypeID(Guid claimTypeId)
        {
            return (manager as ClaimDataManager)?.GetByClaimType(claimTypeId) ?? null;
        }
    }

}
