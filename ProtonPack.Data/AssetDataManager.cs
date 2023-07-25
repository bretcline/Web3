
using ProtonPack.Data;
using SqlSugar;
using System.Configuration;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data
{
    public class AssetDataManager : BaseDataManager<Asset>
    {
        internal AssetDataManager(CompanyUser cu) : base(cu)
        {
        }


        public override ISugarQueryable<Asset> DefaultQuery()
        {
            var rc = base.DefaultQuery()
                .Includes(r => r.AssetOwner)
                .Includes(c => c.ChildAssets)
                .Includes(c => c.IpfsItems)
                ;

            return rc;
        }

        public List<Asset> GetAssetsByUsername(string username)
        {
            var rc = new List<Asset>();

            using var manager = new CustomerDataManager(CompanyUser);

            var user = manager.First(c => c.UserName == username);
            return GetAssetsByOwnerID( user.ID );
        }
       public List<Asset> GetAssetsByWalletAddress(string walletAddress)
        {
            var rc = new List<Asset>();

            //using var manager = new CustomerDataManager(CompanyUser);
            using var manager = DataManagerFactory.GetDataManager<Customer, CustomerDataManager>(CompanyUser);

            var user = manager.First(c => c.WalletAddress == walletAddress);
            return GetAssetsByOwnerID( user.ID );
        }

        public List<Asset> GetByAssetType(Guid assetTypeID)
        {
            var rc = DefaultQuery().Where(a => a.AssetTypeID == assetTypeID).ToList();

            //if (!rc.Any())
            //    throw new WebThreeException("No Assets.");

            return rc;
        }

        public List<Asset> GetAssetsByOwnerID( Guid ownerId )
        {
            var rc = DefaultQuery().Where(a => a.AssetOwnerID == ownerId).ToList();

            if (!rc.Any())
                throw new WebThreeException("No Assets.");

            return rc;
        }

        public List<Asset> GetAllByUserID(string userId)
        {
            throw new NotImplementedException();
        }

        public Asset GetByAssetNumber(string assetNumber)
        {
            return First(c => c.AssetNumber == assetNumber);
        }

        public Asset GetByAssetNumberForAllocation(string assetNumber, Guid customerId)
        {
            var asset = First(c => c.AssetNumber == assetNumber);
            if( Context.Queryable<WalletTransaction>().Any( t => t.AssetID == asset.ID && t.RelatedEntityID == customerId ) )
            {
                // throw new WebThreeException("There is already an allocation for this asset.");
            }
            return asset;
        }

    }
}
