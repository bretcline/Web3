
using ProtonPack.Data;
using SqlSugar;
using System.Configuration;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data
{
    public class ClaimDataManager : BaseDataManager<Claim>
    {
        internal ClaimDataManager(CompanyUser cu) : base(cu)
        {
        }


        public override ISugarQueryable<Claim> DefaultQuery()
        {
            var rc = base.DefaultQuery();
                

            return rc;
        }

        public List<Claim> GetByParentAsset(Guid ParentAssetId)
        {
            var rc = DefaultQuery().Where(a => a.ParentAssetID == ParentAssetId).ToList();

            return rc;
        }
        public List<Claim> GetByClaimType(Guid claimTypeId)
        {
            var rc = DefaultQuery().Where(a => a.ClaimTypeID == claimTypeId).ToList();

            return rc;
        }

    }
}
