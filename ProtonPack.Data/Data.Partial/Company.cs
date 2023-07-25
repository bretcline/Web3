using SqlSugar;

namespace ProtonPack.Data
{
    public partial class Company : TransactionData, IWalletTransactionData
    {
    }

    public partial class EnvironmentData
    {
       public Guid ContractID { get; set; }
       public string EnvironmentImage { get; set; }
       public string EnvironmentName { get; set; }
       public string FullName { get; set; }
       public string HQueueGraphics { get; set; }
       public string PixelStreamURL { get; set; }
       public int QueueSize { get; set; }
       public int QueueTimeout { get; set; }
       public string VQueueGraphics { get; set; }
    }
}
