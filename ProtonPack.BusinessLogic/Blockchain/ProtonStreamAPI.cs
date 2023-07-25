using Nethereum.Web3;
using Nethereum.Contracts;
using Newtonsoft.Json;
using Nethereum.Web3.Accounts;
using Nethereum.RPC.Eth.DTOs;
using ProtonPack.Data;
using Zuul.Data;
using static WebThree.Shared.Utilities;

namespace WebThree.Shared.Blockchain
{
    public class ProtonStreamAPI : BaseProtonSteamAPI
    {
        public string abi { get; private set; }

        public ProtonStreamAPI( CompanyUser cu ) : base("", "", "", 0 )
        {
            var defaultValue = cu.ForceAuthDb;
            cu.ForceAuthDb = true;
            using var manager = Zuul.Data.DataManagerFactory.GetDataManager<ContractSetting, ContractSettingDataManager>(cu);
            var settings = manager.First(s => s.ID == AssetTypes.CryptoPunk);
            cu.ForceAuthDb = defaultValue;

            if (null != settings)
            {
                Initialize( settings.ContractAddress, settings.ChainURL, settings.ChainAPIKey, settings.ChainID );

                if( string.IsNullOrWhiteSpace( settings.Abi ) )
                {
                    settings.Abi = GetABI( );
                    abi = settings.Abi;
                }
            }
        }

        public ProtonStreamAPI(string address, string providerURL, int chainID, string abi) : base(address, providerURL, "", chainID)
        {
            this.abi = abi;
        }

        protected override Contract GenerateUnsignedContract()
        {
            var web3 = new Web3(this.ProviderURL);
            var contract = web3.Eth.GetContract(abi, this.Address);
            return contract;
        }

        protected override void GenerateSignedContract(string signer, out Contract contract, out Account account, out Web3 web3)
        {
            if( !signer.StartsWith( "0x0" ) )
                signer = "0x" + signer;
            account = new Account(signer, this.ChainID);
            web3 = new Web3(account, this.ProviderURL);
            contract = web3.Eth.GetContract(this.abi, this.Address);
        }

        public Web3 GetWeb3Instance()
        {
            return new Web3(this.ProviderURL);
        }

        public Nethereum.Contracts.Event GetEvent(string name)
        {
            var web3 = new Web3(this.ProviderURL);
            var contract = web3.Eth.GetContract(this.abi, this.Address);
            return contract.GetEvent(name);
        }


        T Call<T>(string address, string functionName, string blockchain)
        {
            var result = GetBlockchainInfo(blockchain);

            var api = new ProtonStreamAPI(address, result.Item1, result.Item2, abi);
            return api.Call<T>(functionName).GetAwaiter().GetResult();
        }

        TransactionReceipt CallSigned(string address, string functionName, string signer, string blockchain)
        {
            var result = GetBlockchainInfo(blockchain);

//            var abi = GetABI(result.Item3, address);
            var abi = GetABI();
            var api = new ProtonStreamAPI(address, result.Item1, result.Item2, abi);
            return api.CallSigned(functionName, signer).GetAwaiter().GetResult();
        }

        Tuple<string, int, BaseScanAPIData> GetBlockchainInfo(string blockchain)
        {
            var providerURL = String.Empty;
            int chainID = 0;
            BaseScanAPIData scanData = null;

            if (blockchain == "mumbai")
            {
                var alchemyKey = "";
                var scanKey = "";

                providerURL = "https://polygon-mumbai.g.alchemy.com/v2/" + alchemyKey;
                chainID = 80001;
                scanData = new MumbaiScanAPIData(scanKey);
            }
            else if (blockchain == "matic")
            {
                var alchemyKey = "";
                var scanKey = "";

                providerURL = "https://polygon-mainnet.g.alchemy.com/v2/" + alchemyKey;
                chainID = 137;
                scanData = new PolygonScanAPIData(scanKey);
            }
            else if (blockchain == "eth")
            {
                var alchemyKey = "";
                var ethKey = "";

                providerURL = "https://eth-mainnet.g.alchemy.com/v2/" + alchemyKey;
                chainID = 1;
                scanData = new EtherscanAPIData(ethKey);
            }

            return new Tuple<string, int, BaseScanAPIData>(providerURL, chainID, scanData);
        }


        BaseScanAPIData GetScanData(int chainID)
        {
            BaseScanAPIData scanData = null;

            var scanKey = string.Empty;
    
            switch( chainID )
            {
                case 1:
                    {
                        scanData = new EtherscanAPIData(scanKey);
                        break;
                    }
                case 137:
                    {
                        scanData = new PolygonScanAPIData(scanKey);
                        break;
                    }
                case 80001:
                    {
                        scanData = new MumbaiScanAPIData(scanKey);
                        break;
                    }
            }
            return scanData;
        }


        string GetABI()
        {
            var scanData = GetScanData( ChainID );
            var abiLink = scanData.GetABIURL(Address);

            using HttpClient client = new();
            var responseString = client.GetStringAsync(abiLink).GetAwaiter().GetResult();
            var obj = JsonConvert.DeserializeObject<CustomResponse>(responseString);
            var abi = obj?.result ?? String.Empty;
            return abi;
        }
    }

    public class CustomResponse
    {
        public string result;
    }
}
