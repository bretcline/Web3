using SqlSugar;

namespace ProtonPack.Data
{
    public partial class Asset
    {
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(AssetOwnerID))] 
        public Customer AssetOwner { get; set; }

        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(ParentAssetID))]
        public List<Asset> ChildAssets { get; set; }

        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(IpfsItem.AssetID))]
        public List<IpfsItem> IpfsItems { get; set; }

    }
}
