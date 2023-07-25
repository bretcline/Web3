using ProtonPack.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebThree.Shared.Utilities;


namespace ProtonPack.BusinessLogic
{
    public class MediaManager : BaseBusinessLogicManager<Media>
    {
        public MediaManager(CompanyUser companyUser) : base(companyUser) { }

        public List<Media> GetAllByParentAssetID(Guid parentAssetId)
        {
            return (manager as MediaDataManager)?.GetByParentAsset(parentAssetId) ?? new List<Media>();
        }
        public List<Media> GetAllByMediaTypeID(Guid MediaTypeId)
        {
            return (manager as MediaDataManager)?.GetByMediaType(MediaTypeId) ?? null;
        }
    }

}
