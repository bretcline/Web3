using SqlSugar;

namespace ProtonPack.Data
{
    public partial class Customer : TransactionData, IWalletTransactionData
    {
        [SugarColumn(IsIgnore = true)]
        public Group Group { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string GroupName => Group?.GroupName ?? "Unknown";

        [SugarColumn(IsIgnore = true)]
        public List<Asset> Assets { get; set; }
    }
}
