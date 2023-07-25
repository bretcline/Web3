using SqlSugar;

namespace ProtonPack.Data
{

    public interface IWalletTransactionData
    {
        Guid ID { get; set; }
        string WalletAddress { get; set; }
        decimal WalletBalance { get; set; }
        Guid AssetID { get; set; }
        Guid TransactionID { get; set; }
        Guid RuleID { get; set; }

    }


    public class TransactionData
    {
        [SugarColumn(IsIgnore = true)]
        public Guid AssetID { get; set; }

        [SugarColumn(IsIgnore = true)]
        public Guid TransactionID { get; set; }

        [SugarColumn(IsIgnore = true)]
        public Guid RuleID { get; set; }
    }
}
//export interface Root
//{
//    mediaInteractions: number
//    currentBalance: number
//    chiveMoney: number
//    userNFTs: number
//    status: number
//    tokens: number
//    leaderboardUsers: LeaderboardUser[]
//    chiveStock: ChiveStock[]
//}

//export interface LeaderboardUser
//{
//    id: number
//    name: string
//    personalStatus: number
//}

//export interface ChiveStock
//{
//    date: string
//    close: number