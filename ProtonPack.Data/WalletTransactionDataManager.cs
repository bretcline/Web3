using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data
{
    public class WalletTransactionDataManager : BaseDataManager<WalletTransaction>
    {
        internal WalletTransactionDataManager(CompanyUser cu) : base(cu)
        {
        }

        internal void CreateTransaction(IWalletTransactionData entity, IWalletTransactionData item, bool treasuryTransaction)
        {
            var diff = entity.WalletBalance - item.WalletBalance;

            var transaction = Create();
            transaction.RelatedEntityID = item.ID;
            transaction.TreasuryTransaction = treasuryTransaction;
            transaction.TransactionAmount = diff;
            transaction.WalletBalance = entity.WalletBalance;
            transaction.WalletAddress = entity.WalletAddress;
            transaction.TransactionID = entity.TransactionID;
            transaction.RuleID = entity.RuleID;
            transaction.AssetID = entity.AssetID;
            Add(transaction);
        }
    }
}