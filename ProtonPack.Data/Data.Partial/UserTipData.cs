namespace ProtonPack.Data
{
    public class UserTipData
    {
        public Guid CompanyID { get; set; }
        public string FromUserID { get; set; }
        public string ToUserID { get; set; }
        public string AssetID { get; set; }
        public decimal Amount { get; set; }
    }
}
