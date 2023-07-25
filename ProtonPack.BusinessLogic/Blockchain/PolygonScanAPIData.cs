namespace WebThree.Shared.Blockchain
{
    public class PolygonScanAPIData : BaseScanAPIData
    {
        public PolygonScanAPIData(string apiKey) : base(apiKey)
        {

        }

        public override string GetBaseURL()
        {
            return "polygonscan.com";
        }
    }

}
