namespace WebThree.Shared.Blockchain
{
    public abstract class BaseScanAPIData : BaseBlockchainAPIData
    {
        public BaseScanAPIData(string apiKey) : base(apiKey)
        {

        }

        public override string GetURL()
        {
            return "https://api" + this.GetFinalChainExtension + "." + this.GetBaseURL() + "/api";
        }

        public string GetFinalChainExtension
        {
            get
            {
                if (this.GetPossibleChainExtension != String.Empty)
                {
                    return "-" + this.GetPossibleChainExtension;
                }

                return this.GetPossibleChainExtension;
            }

        }

        public virtual string GetPossibleChainExtension => String.Empty;

        public string GetABIURL(string address)
        {
            return this.GetURL() + "?module=contract&action=getabi&address=" + address + "&apikey=" + this.apiKey;
        }
    }

}
