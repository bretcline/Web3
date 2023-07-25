using ProtonPack.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebThree.Shared.Utilities;


namespace ProtonPack.BusinessLogic
{
    public class AssetManager : BaseBusinessLogicManager<Asset>
    {
        public AssetManager(CompanyUser companyUser) : base(companyUser) { }

        public List<Asset> GetAllByUserID(string userId)
        {
            return (manager as AssetDataManager)?.GetAssetsByUsername( userId ) ?? new List<Asset>();
        }
        public List<Asset> GetAllByWalletAddress(string walletAddress)
        {
            return (manager as AssetDataManager)?.GetAssetsByWalletAddress( walletAddress ) ?? new List<Asset>();
        }

        public Asset GetByAssetNumber(string assetNumber)
        {
            return (manager as AssetDataManager)?.GetByAssetNumber(assetNumber) ?? null;
        }
        public List<Asset> GetByAssetType(Guid assetTypeId)
        {
            return (manager as AssetDataManager)?.GetByAssetType(assetTypeId) ?? null;
        }
    }

}
