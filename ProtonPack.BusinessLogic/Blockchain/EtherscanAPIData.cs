namespace WebThree.Shared.Blockchain
{
    public class EtherscanAPIData : BaseScanAPIData
    {
        public EtherscanAPIData(string apiKey) : base(apiKey)
        {

        }

        public override string GetBaseURL()
        {
            return "etherscan.io";
        }
    }

}
