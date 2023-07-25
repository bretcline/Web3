using SqlSugar;

namespace ProtonPack.Data
{
    public partial class WalletTransaction
    {
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(WalletTransaction.RuleID))]
        public Rule Rule { get; set; }

    }
}
