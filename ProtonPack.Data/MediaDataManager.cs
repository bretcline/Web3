
using ProtonPack.Data;
using SqlSugar;
using System.Configuration;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data
{
    public class MediaDataManager : BaseDataManager<Media>
    {
        internal MediaDataManager(CompanyUser cu) : base(cu)
        {
        }


        public override ISugarQueryable<Media> DefaultQuery()
        {
            var rc = base.DefaultQuery();
                

            return rc;
        }

        public List<Media> GetByParentAsset(Guid ParentAssetId)
        {
            var rc = DefaultQuery().Where(a => a.ParentAssetID == ParentAssetId).ToList();

            return rc;
        }
        public List<Media> GetByMediaType(Guid MediaTypeId)
        {
            var rc = DefaultQuery().Where(a => a.MediaTypeID == MediaTypeId).ToList();

            return rc;
        }

    }
}
