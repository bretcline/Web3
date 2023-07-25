using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtonPack.Data
{


    public class CompanySummary
    {
        public string WalletAddress { get; set; }
        public int MediaInteractions { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal TokenBalance { get; set; }
        public int NftBalance { get; set; }
        public int Status { get; set; }

        public List<DataPoint> LikeData { get; set; }
        public List<DataPoint> CommentData { get; set; }
        public List<DataPoint> RedemptionData { get; set; }

        public List<DataPoint> TransactionHistory { get; set; }

    }


    public class CustomerSummary
    {
        public string WalletAddress { get; set; }
        public int MediaInteractions { get;set; }
        public decimal CurrentBalance { get; set; }
        public decimal TokenBalance { get; set; }
        public int NftBalance { get; set; }
        public int Status { get; set; }

        public List<LeaderBoardUser> LeaderboardUsers { get; set; }
        public List<DataPoint> LikeData { get; set; }
        public List<DataPoint> CommentData { get; set; }
        public List<DataPoint> TipData { get; set; }

        public List<DataPoint> TransactionHistory { get; set; }

    }

    public class DataPoint
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public Guid BehaviorTypeID { get; set; }
        public string BehaviorType { get; set; }
    }

    public class LeaderBoardUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
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