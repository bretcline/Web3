namespace WebThree.Shared.Blockchain
{
    public class MumbaiScanAPIData : PolygonScanAPIData
    {
        public MumbaiScanAPIData(string apiKey) : base(apiKey)
        {

        }

        public override string GetPossibleChainExtension => "testnet";
    }

}
