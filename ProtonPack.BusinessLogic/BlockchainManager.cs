using ProtonPack.Data;
using static WebThree.Shared.Utilities;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;
using Zuul.Data;
using DataManagerFactory = ProtonPack.Data.DataManagerFactory;
using System.Diagnostics;
using WebThree.Shared.Data;
using WebThree.Shared;
using Newtonsoft.Json;
using Ipfs.Http;
using Pinata.Client;
using ProtonPack.BusinessLogic.Data;
using System.Net;
using Google.Protobuf.WellKnownTypes;
using SqlSugar;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace ProtonPack.BusinessLogic
{
    public class WalletBalanceData
    {
        public WalletBalanceData() { OwnedAssets = new List<UiAsset> { }; }
        public Guid CompanyId { get; set; }
        public string WalletAddress { get; set; }
        public BigInteger WalletBalance { get; set; }
        public List<string> Metadata { get; set; }

        public List<UiAsset> OwnedAssets { get; set; }

    }

    public class UiAsset
    {
        public UiAsset()
        {
            ChildAssets = new List<UiAsset>();
            ClaimAssets = new List<UiClaim>();
            MediaAssets = new List<UiMedia>();
            BurnBMAssets = new List<UiAsset>();
            BurnTixAssets = new List<UiAsset>();
            IpfsItems = new List<IpfsItem>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IpfsImage { get; set; }
        public string Url { get; set; }
        public Guid TypeID { get; set; }
        public Guid AssetID { get; set; }
        public Guid? AssetOwnerID { get; set; }
        public string AssetNumber { get; set; }
        public List<UiAsset>? ChildAssets { get; set; }
        public List<UiAsset>? BurnBMAssets { get; set; }
        public List<UiAsset>? BurnTixAssets { get; set; }
        public List<UiClaim>? ClaimAssets { get; set; }
        public List<UiMedia>? MediaAssets { get; set; }
        public List<IpfsItem>? IpfsItems { get; set; }
        public string ContractAddress { get; set; }
        public int ContractType { get; set; }
    }
    public class CryptoAddress
    {
        public string WalletAddress { get; set; }
        public bool OwnershipData { get; set; }
        public List<Guid> Contracts { get; set; }
    }

    public class UiClaim
    {
        public UiClaim() { }

        public string Name { get; set; }
        public string Url { get; set; }
        public Guid? ClaimTypeID { get; set; }
        public Guid? ParentAssetID { get; set; }
        public string Code { get; set; }
        public bool Claimed { get; set; }
        public Guid? ContractTypeID { get; set; }
        public string TokenID { get; set; }
    }

    public class UiMedia
    {
        public UiMedia() { }

        public string Name { get; set; }
        public string Url { get; set; }
        public Guid? MediaTypeID { get; set; }
        public Guid? ParentAssetID { get; set; }
        public Guid? ContractTypeID { get; set; }
        public string? Description { get; set; }
    }


    public class BlockchainManager
    {

        private object locker = new object();

        private List<Asset> m_Failed = new List<Asset>();
        private static int PageSize = 12;

        private CompanyUser cu;

        public BlockchainManager(CompanyUser cu)
        {
            this.cu = cu;
        }

        public UserSession CheckAdministrativeRights(string walletAddress, Guid companyId, Guid contractType)
        {
            // Hack for now...
            using var manager = DataManagerFactory.GetDataManager<Customer, CustomerDataManager>(cu);
            var customer = manager.GetByWalletAddress(walletAddress);

            using var aManager = DataManagerFactory.GetDataManager<Asset, AssetDataManager>(cu);
            var assets = aManager.GetAssetsByOwnerID(customer.ID);

            // this is bit of a hack for TheForever...
            if (assets.Select(a => Convert.ToInt32(a.AssetNumber) >= 10000).Any())
            {
                cu.ForceAuthDb = true;
                using var sessionManager = Zuul.Data.DataManagerFactory.GetDataManager<UserSession, UserSessionDataManager>(cu);
                using var userManager = Zuul.Data.DataManagerFactory.GetDataManager<User, UserDataManager>(cu);

                var admin = userManager.GetAll().FirstOrDefault(u => u.AdminUser && u.CompanyID == cu.CompanyId);

                var userSession = sessionManager.Create();
                userSession.UserID = admin?.ID ?? Guid.Empty;
                userSession.StartDate = userSession.LastUpdated = DateTime.UtcNow;
                cu.ForceAuthDb = false;

                return sessionManager.Add(userSession);
            }
            else
            {
                throw new WebThreeException();
            }
        }

        public void UpdateEnvironmentData()
        {

        }


        public Guid AuthenticateAndQueue(string walletAddress, Guid companyID, Guid AssetType)
        {
            //using var manager = Zuul.Data.DataManagerFactory.GetDataManager<ContractSetting, ContractSettingDataManager>(cu);
            //var settings = manager.First(s => s.ID == AssetType);

            //if (null != settings)
            {
                // Call Bockchain Logic to get total supply.
                if (AssetType == Guid.Empty || AssetType == AssetTypes.CryptoPunk)
                {
                    using var manager = DataManagerFactory.GetDataManager<Customer, CustomerDataManager>(cu);
                    var customer = manager.GetByWalletAddress(walletAddress);
                    if (customer != null)
                    {
                        using var assetManager = DataManagerFactory.GetDataManager<Asset, AssetDataManager>(cu);

                        var assets = assetManager.GetAssetsByUsername(customer.UserName);

                        using var queueManager = DataManagerFactory.GetDataManager<Queue, QueueDataManager>(cu);

                        if (!queueManager.CheckQueueByCustomerID(customer.ID))
                        {
                            var q = queueManager.Create();

                            q.CustomerID = customer.ID;
                            q.ContractTypeID = AssetType;
                            q.QueueData = $"{{\"WalletAddress\": \"{walletAddress}\", \"AssetId\": \"{assets[0].AssetNumber}\" , \"AssetData\": \"{assets[0].AssetData}\" }}";

                            queueManager.Add(q);

                            return q.ID;
                        }
                        else
                            throw new WebThreeException("Already in the queue.");
                    }
                    else
                        throw new WebThreeException("Invalid Customer.");
                }
                else
                    throw new WebThreeException("Invalid Contract.");
            }
        }

        public void QueueCleanup()
        {
            using var queueManager = DataManagerFactory.GetDataManager<Queue, QueueDataManager>(cu);

            queueManager.QueueCleanup();
        }

        public int GetQueuePosition(Guid queueID)
        {
            using var queueManager = DataManagerFactory.GetDataManager<Queue, QueueDataManager>(cu);

            return queueManager.GetQueuePosition(queueID);
        }

        public Guid GetTopToken()
        {
            using var queueManager = DataManagerFactory.GetDataManager<Queue, QueueDataManager>(cu);
            return queueManager.GetTopToken();
        }


        public bool UpdateOwner(string assetNumber, string walletAddress)
        {
            bool rc = false;
            using var manager = new AssetManager(cu);
            var asset = manager.GetByAssetNumber(assetNumber);
            if (null != asset)
            {
                using var custManager = new CustomerManager(cu);
                var customer = custManager.GetByWalletAddress(walletAddress);
                if (null == customer)
                {
                    customer = custManager.Create();
                    customer.CompanyID = cu.CompanyId;
                    customer.WalletAddress = walletAddress;
                    customer.UserName = walletAddress;

                    customer = custManager.Add(customer).GetAwaiter().GetResult();
                }
                if (null != customer)
                {
                    asset.AssetOwnerID = customer.ID;
                    manager.Update(asset).GetAwaiter().GetResult();

                    rc = true;
                }
            }
            return rc;
        }

        public string VerifyQueueItem(Guid queueToken)
        {
            using var queueManager = DataManagerFactory.GetDataManager<Queue, QueueDataManager>(cu);

            var token = queueManager.Get(queueToken);
            var position = queueManager.GetQueuePosition(queueToken);
            if (0 == position)
            {
                if (null != token)
                {
                    token.Verified = true;
                    queueManager.Update(token);
                    return token.QueueData;
                }
                else
                    throw new WebThreeException("Invalid Token.");
            }
            else
                throw new WebThreeException("It's not your turn.");
        }

        public bool DequeueToken(Guid queueToken)
        {
            using var queueManager = DataManagerFactory.GetDataManager<Queue, QueueDataManager>(cu);

            var token = queueManager.Get(queueToken);
            if (null != token)
            {
                queueManager.Delete(token.ID);
                return true;
            }
            else
                throw new WebThreeException("Invalid Token.");
        }


        public Task<bool> ManualDequeue(bool clearAll)
        {
            return Task.Run(() =>
            {
                using var queueManager = DataManagerFactory.GetDataManager<Queue, QueueDataManager>(cu);

                if (true == clearAll)
                {
                    queueManager.ClearAll();
                }
                else
                {
                    queueManager.ClearNext();
                }
                return true;
            });

        }

        [Function("balanceOf", "uint256")]
        public class BalanceOfFunction : FunctionMessage
        {
            [Parameter("address", "owner")]
            public string Owner { get; set; }
        }

        // The owner function message definition
        [Function("ownerOf", "address")]
        public class OwnerOfFunction : FunctionMessage
        {
            [Parameter("uint256", "tokenId")]
            public BigInteger TokenId { get; set; }
        }

        public void WalkCryptoPunks()
        {
            var erroredAssets = new List<int>();

            var timer = new Stopwatch();
            timer.Start();

            cu.ForceAuthDb = true;
            using var manager = Zuul.Data.DataManagerFactory.GetDataManager<ContractSetting, ContractSettingDataManager>(cu);
            var settings = manager.First(s => s.ID == AssetTypes.CryptoPunk);
            cu.ForceAuthDb = false;

            if (null != settings)
            {

                var web3 = new Web3($"{settings.ChainURL}{settings.ChainAPIKey}");

                var erc721TokenContractAddress = settings.ContractAddress;

                var contract = web3.Eth.GetContract(settings.Abi, settings.ContractAddress);
                var totalFunction = contract.GetFunction("totalSupply");
                var punkIndexToAddress = contract.GetFunction("punkIndexToAddress");
                var totalSupply = totalFunction.CallAsync<int>().GetAwaiter().GetResult();

                using var aManager = DataManagerFactory.GetDataManager<Asset, AssetDataManager>(cu);

                using var cManager = DataManagerFactory.GetDataManager<Customer, CustomerDataManager>(cu);
                var customers = cManager.GetAll();

                var webTimer = new Stopwatch();

                var taskList = new List<Task<CustomerAsset>>();

                //                for ( int i = 0; i < totalSupply; i++ )
                Console.WriteLine("Begin tasks");
                var allDone = new List<CustomerAsset>();
                //int counter = 0;

                var totalRecords = aManager.RecordCount();
                var recordBucket = totalRecords / PageSize;
                var leftovers = totalRecords - (recordBucket * PageSize);

                var allAssets = new List<Asset>();
                for (var i = 0; i < recordBucket + 1; i++)
                {
                    var assets = aManager.GetPaged(i, PageSize);
                    allAssets.AddRange(assets.Data.Where(a => Convert.ToInt32(a.AssetNumber) < totalSupply));
                    List<Asset> updatedAssets = ProcessItems(punkIndexToAddress, totalSupply, aManager, cManager, customers, taskList, allDone, assets.Data, settings);

                    erroredAssets.AddRange(allDone.Where(c => c.Error != null && c.Error.Length > 0).Select(c => Convert.ToInt32(c.AssetNumber)).ToList());

                    Console.WriteLine($"tasks added - {i}, updated {updatedAssets.Count}, total errors: {erroredAssets.Count}");

                    allDone.Clear();
                    taskList.Clear();
                    Thread.Sleep(150);
                }
                WalkErrors(punkIndexToAddress, aManager, cManager, customers, allAssets, erroredAssets, settings);
            }
            timer.Stop();
            Console.WriteLine($"Elapsed Time: {timer.ElapsedMilliseconds / 1000}");
        }

        public void WalkContract(Guid contractId)
        {
            var timer = new Stopwatch();
            timer.Start();

            cu.ForceAuthDb = true;
            using var manager = Zuul.Data.DataManagerFactory.GetDataManager<ContractSetting, ContractSettingDataManager>(cu);
            var settings = manager.First(s => s.ID == contractId);
            cu.ForceAuthDb = false;

            if (null != settings)
            {
                var web3 = new Web3($"{settings.ChainURL}{settings.ChainAPIKey}");

                var erc721TokenContractAddress = settings.ContractAddress;

                var contract = web3.Eth.GetContract(settings.Abi, settings.ContractAddress);
                var totalFunction = contract.GetFunction("totalSupply");
                var punkIndexToAddress = contract.GetFunction("ownerOf");
                var totalSupply = totalFunction.CallAsync<int>().GetAwaiter().GetResult();
                Console.WriteLine($"totalSupply: {totalSupply}");

                using var aManager = DataManagerFactory.GetDataManager<Asset, AssetDataManager>(cu);
                var assets = aManager.GetByAssetType(contractId);
                Console.WriteLine($"Assets Count: {assets.Count}");

                if (null == assets || false == assets.Any() || assets.Count < totalSupply)
                {
                    //var startIndex = 0;

                    var startIndex = settings.ContractMinterType == 1 ? 0 : 1; // Hypermint ERC721 index start at 1
                    if (null == assets)
                        assets = new List<Asset>();
                    else if (assets.Count < totalSupply)
                        startIndex = assets.Count;

                    for (var i = startIndex; i < totalSupply + 1; i++)
                    {
                        var newAsset = aManager.Create();
                        newAsset.AssetNumber = i.ToString();
                        newAsset.AssetTypeID = contractId;
                        //newAsset.Status = "";
                        assets.Add(aManager.Add(newAsset));
                    }
                }

                ProcessAssetList(settings, punkIndexToAddress, totalSupply, aManager, assets);
                if (m_Failed.Any())
                {
                    var failed = new List<Asset>();
                    failed.AddRange(m_Failed);
                    ProcessAssetList(settings, punkIndexToAddress, totalSupply, aManager, failed);
                }
            }
            timer.Stop();
            Console.WriteLine($"Elapsed Time: {timer.ElapsedMilliseconds / 1000}");
        }

        private void ProcessAssetList(ContractSetting settings, Function punkIndexToAddress, int totalSupply, AssetDataManager aManager, List<Asset>? assets)
        {
            var cManager = DataManagerFactory.GetDataManager<Customer, CustomerDataManager>(cu);
            var customers = cManager.GetAll();

            var webTimer = new Stopwatch();

            var taskList = new List<Task<CustomerAsset>>();

            //                for ( int i = 0; i < totalSupply; i++ )
            Console.WriteLine("Begin tasks");
            var ordered = assets.OrderBy(a => a.AssetNumber);

            foreach (var asset in ordered)
            {
                try
                {
                    var i = Convert.ToInt32(asset.AssetNumber);
                    if (i <= totalSupply)
                    {
                        Thread.Sleep(250);
                        taskList.Add(ProcessAsset(punkIndexToAddress, aManager, cManager, customers, asset, settings));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{asset.AssetNumber} - {ex.ToString()}");
                }
            }
            Console.WriteLine("All tasks added");
            var allDone = Task.WhenAll(taskList).GetAwaiter().GetResult();

            var newCustomers = allDone.Where(c => c.Customer != null).Select(c => c.Customer).ToList();
            cManager.BulkAdd(newCustomers);

            var updatedAssets = allDone.Where(c => c.Asset != null).Select(c => c.Asset).ToList();
            foreach (var asset in updatedAssets)
            {
                aManager.Update(asset);
            }
        }

        public class CustomerAsset
        {
            public string WalletAddress { get; set; }
            public string AssetNumber { get; set; }
            public Customer Customer { get; set; }
            public Asset Asset { get; set; }

            public string Error { get; set; }

        }



        private void WalkErrors(Function punkIndexToAddress, AssetDataManager aManager, CustomerDataManager cManager, List<Customer> customers, List<Asset> assets, List<int> assetNumbers, ContractSetting settings, int recursionCount = 0)
        {
            if (recursionCount < 5)
            {
                Console.WriteLine($"processing {assetNumbers.Count} missing items.");
                var allDone = new List<CustomerAsset>();
                var erroredAssets = new List<int>();
                var taskList = new List<Task<CustomerAsset>>();
                var assetList = assets.Where(a => assetNumbers.Contains(Convert.ToInt32(a.AssetNumber))).ToList();

                if (assetList.Count > PageSize)
                {
                    var assetLists = assetList.SplitList(assetList.Count / PageSize);
                    foreach (var aList in assetLists)
                    {
                        Console.WriteLine($"processing {aList.Count} sub items.");
                        ProcessItems(punkIndexToAddress, assets.Count, aManager, cManager, customers, taskList, allDone, aList, settings);
                    }
                }
                else
                {
                    ProcessItems(punkIndexToAddress, assets.Count, aManager, cManager, customers, taskList, allDone, assetList, settings);
                }




                erroredAssets.AddRange(allDone.Where(c => c.Error != null && c.Error.Length > 0).Select(c => Convert.ToInt32(c.AssetNumber)).ToList());

                WalkErrors(punkIndexToAddress, aManager, cManager, customers, assets, erroredAssets, settings, ++recursionCount);
            }
        }


        private List<Asset> ProcessItems(Function punkIndexToAddress, int totalSupply, AssetDataManager aManager, CustomerDataManager cManager, List<Customer> customers, List<Task<CustomerAsset>> taskList, List<CustomerAsset> allDone, List<Asset> assets, ContractSetting settings)
        {
            foreach (var asset in assets)
            {
                try
                {
                    var assetNum = Convert.ToInt32(asset.AssetNumber);
                    if (assetNum < totalSupply)
                    {
                        taskList.Add(ProcessAsset(punkIndexToAddress, aManager, cManager, customers, asset, settings));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            allDone.AddRange(Task.WhenAll(taskList).GetAwaiter().GetResult());
            try
            {
                var newCustomers = allDone.Where(c => c.Customer != null).Select(c => c.Customer).ToList();
                cManager.BulkAdd(newCustomers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var updatedAssets = allDone.Where(c => c.Asset != null).Select(c => c.Asset).ToList();
            foreach (var asset in updatedAssets)
            {
                aManager.Update(asset);
            }

            return updatedAssets;
        }


        private Task<CustomerAsset> ProcessAsset(Function punkIndexToAddress, AssetDataManager aManager, CustomerDataManager cManager, List<Customer> customers, Asset asset, ContractSetting settings)
        {
            var task = new Task<CustomerAsset>(() =>
            {
                var ca = new CustomerAsset
                {
                    AssetNumber = asset.AssetNumber
                };

                try
                {
                    var assetNum = Convert.ToInt32(asset.AssetNumber);
                    var address = punkIndexToAddress.CallAsync<string>(assetNum).GetAwaiter().GetResult();
                    if (address != null)
                    {
                        AssignOwner(cManager, customers, asset, ca, address);
                        UpdateIPFS(asset, settings); // Uncomment to Update Assets
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{asset.AssetNumber} - {ex.ToString()}");
                    ca.Error = ex.ToString();
                }
                return ca;
            });
            task.Start();
            return task;
        }

        // The owner function message definition
        [Function("tokenURI", "string")]
        public class TokenURIFunction : FunctionMessage
        {
            [Parameter("uint256", "tokenId", 1)]
            public BigInteger TokenId { get; set; }
        }

        private void UpdateIPFS(Asset asset, ContractSetting settings)
        {
            string uri = string.Empty;
            try
            {
                var tokenUriMessage = new TokenURIFunction() { TokenId = Convert.ToInt32(asset.AssetNumber) };

                var web3 = new Web3($"{settings.ChainURL}{settings.ChainAPIKey}");

                var uriHandler = web3.Eth.GetContractQueryHandler<TokenURIFunction>();

                uri = uriHandler
                    .QueryAsync<string>(settings.ContractAddress, tokenUriMessage)
                    .ConfigureAwait(false).GetAwaiter().GetResult();


                if (0 == uri.CompareTo("https://projectvenkman.mypinata.cloud/ipfs/Qmck35Df4SovEiymADYv5jyaXxj2YquAo2RX4zJkbytMDX"))
                {
                    Console.Write("Green Curtain  ");
                }
                else
                {
                    bool hypermintUri = uri.IndexOf("hypermint") > -1;
                    string jsonString = "";

                    if (!hypermintUri)
                    {
                        string filename = uri.Remove(0, uri.IndexOf("/ipfs/") + "/ipfs/".Length);
                        filename = filename.Replace("ipfs", "").TrimStart(':').TrimStart('/');
                        var ipfs = new IpfsClient("https://ipfs.io");
                        jsonString = ipfs.FileSystem.ReadAllTextAsync(filename).GetAwaiter().GetResult();
                    }
                    else
                    {
                        Thread.Sleep(10000);
                        var http = new HttpClient();
                        jsonString = http.GetStringAsync(uri).GetAwaiter().GetResult();
                    }

                    Thread.Sleep(10000);

                    var item = System.Text.Json.JsonSerializer.Deserialize<IpfsItem>(jsonString);
                    if (null != item)
                    {
                        if (null != item.Attributes && item.Attributes.Count > 0) // Uncomment to update Attributes
                        {
                            item.OnChain = true;
                            item.ItemIndex = asset.AssetNumber;

                            var manager = DataManagerFactory.GetDataManager<IpfsItem, IpfsItemDataManager>(cu);

                            var entity = manager.GetByItemIndex(item.ItemIndex);
                            if (entity == null)
                            {
                                var iname = item.ItemName != null ? item.ItemName : "";
                                entity = manager.GetByName(iname);
                            }

                            if (string.IsNullOrWhiteSpace(item.ItemImage))
                                item.ItemImage = uri;

                            if (null != asset)
                                item.AssetID = asset.ID;

                            if (null == entity)
                            {
                                manager.Add(item);
                                Console.WriteLine($"Added: {item.ItemIndex} - {item.ItemName}");
                            }
                            else
                            {
                                manager.Update(item, entity);
                                Console.WriteLine($"Updated: {item.ItemIndex} - {item.ItemName}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_Failed.Add(asset);
                Console.WriteLine($"{asset.AssetNumber} - {uri}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }

        private void AssignOwner(CustomerDataManager cManager, List<Customer> customers, Asset asset, CustomerAsset ca, string address)
        {
            var customer = customers.FirstOrDefault(c => c.WalletAddress == address);
            if (null == customer)
            {
                customer = cManager.Create();
                customer.UserName = address;
                customer.WalletAddress = address;
                customer.GroupID = Guid.Empty;
                customer.CompanyID = cu.CompanyId;
                ca.Customer = customer;
                customers.Add(customer);
            }
            if (address == "0x7F90F772d4DAfb54601dfA4D6022F2542a409C98")
            {
                asset.Status = "WebThree";
            }
            else if (address == "0x8697e2b04b16effdf399ff1bc7d42663709a8851")
            {
                asset.Status = "Hypermint";
            }
            else
            {
                asset.Status = "Owned";
            }
            if (asset.AssetOwnerID != customer.ID)
            {
                asset.AssetOwnerID = customer.ID;
                ca.Asset = asset;
            }
            Console.WriteLine($"Assign Owner: {asset.AssetNumber}");
        }

        public List<WalletBalanceData> VerifyOwnership(CryptoAddress wallet, Guid companyId, Guid contractType)
        {
            var SpoofedWallet = string.Empty;
            // TESTING FOR JOHNNY
            List<string> venkmanWallets = new List<string>();
            venkmanWallets.Add("0xDF7159A4576dEc89b681E4F3404F9FE3133f05F4"); // Johnny MetaMask 
            venkmanWallets.Add("0x7F3507BB654eB76C31263604B7B3A7921229e72f"); // Johnny Coinbase
            venkmanWallets.Add("0x133084f0646006760b68E19a02338634fC11E36F"); // Mike MetaMask
            venkmanWallets.Add("0x42017df7ce71AD2Fe80cCa4C3D9bFc0512fff5Cf"); // Kalvin MetaMask
            //if (wallet.WalletAddress.Equals("0xDF7159A4576dEc89b681E4F3404F9FE3133f05F4", StringComparison.InvariantCultureIgnoreCase))
            if (venkmanWallets.Contains(wallet.WalletAddress))
            {
                //wallet.WalletAddress = "0xc8FC1962214E0B5fbaF0E707c503CA22cE93731E"; //Allen
                //wallet.WalletAddress = "0xf1daDB9488cDB4B8EAf81E24B3156dF64c9c7D2a"; //JabberJaws
                wallet.WalletAddress = "0x2611b286994571b4d5292acff5619da8074b5c54"; //Gavin
                //wallet.WalletAddress = "0x3cE09E7B647DB3580D77826282cC1DEd67EE2846"; //SlickRick
                //wallet.WalletAddress = "0xfA625Cf327e4D2733e1d9FeC37dBa1faF4E0B92c"; //JamesDrobny

                SpoofedWallet = wallet.WalletAddress;
            }
            List<Guid> contracts = wallet.Contracts;

            using var manager = Zuul.Data.DataManagerFactory.GetDataManager<ContractSetting, ContractSettingDataManager>(cu);

            List<WalletBalanceData> balanceData = new List<WalletBalanceData>();

            BigInteger nftCount = 0;

            contracts.ForEach(c =>
            {
                //var settings = manager.First(s => s.ID == contractType);
                var settings = manager.First(s => s.ID == c);
                if (null != settings)
                {

                    var web3 = new Web3($"{settings.ChainURL}{settings.ChainAPIKey}");

                    var erc721TokenContractAddress = settings.ContractAddress;

                    // Send query to contract, to find out how many tokens the owner has
                    var balanceOfMessage = new BalanceOfFunction() { Owner = wallet.WalletAddress };
                    var queryHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
                    var balance = queryHandler
                        .QueryAsync<BigInteger>(erc721TokenContractAddress, balanceOfMessage)
                        .ConfigureAwait(false).GetAwaiter().GetResult();

                    if (0 < balance)
                    {
                        var contract = web3.Eth.GetContract(settings.Abi, settings.ContractAddress);
                        var totalFunction = contract.GetFunction("totalSupply");
                        var totalSupply = totalFunction.CallAsync<int>().GetAwaiter().GetResult();

                        WalletBalanceData temp = WalkErc721Items(wallet, settings, web3, erc721TokenContractAddress, balance, totalSupply);

                        balanceData.Add(temp);

                        // TODO....
                        //switch ( (ErcType) settings.ContractType )
                        //{
                        //    case ErcType.CryptoPunk:
                        //        {
                        //                break;
                        //        }
                        //    case ErcType.Erc721:
                        //        {
                        //            return WalkErc721Items(walletAddress, contractType, web3, erc721TokenContractAddress, balance, totalSupply);
                        //        }

                        //}
                    }
                    nftCount += balance;

                }
                else
                    throw new WebThreeException("Invalid Contract Type.");

            });

            if (nftCount < 1)
            {
                throw new WebThreeException("NFT not contained in wallet!");
            }

            // TESTING FOR JOHNNY
            if (!string.IsNullOrWhiteSpace(SpoofedWallet))
            {
                wallet.WalletAddress = SpoofedWallet;
                foreach (var b in balanceData) { b.WalletAddress = SpoofedWallet; }
            }

            return balanceData;
        }

        private void LoadOwnedAssets(CryptoAddress wallet, WalletBalanceData walletBalance)
        {
            throw new NotImplementedException();
        }

        private Customer GetCustomer(CustomerDataManager custManager, string walletAddress)
        {
            var customer = custManager.First(c => c.UserName == walletAddress) ?? custManager.First(c => c.WalletAddress == walletAddress);
            if (null == customer)
            {
                customer = custManager.Create();
                customer.UserName = walletAddress;
                customer.WalletAddress = walletAddress;
                customer.GroupID = Guid.Empty;
                customer.CompanyID = cu.CompanyId;
                custManager.Add(customer);
            }
            return customer;
        }

        private WalletBalanceData WalkErc721Items(CryptoAddress wallet, ContractSetting contract, Web3 web3, string erc721TokenContractAddress, BigInteger balance, int totalSupply)
        {
            var rc = new WalletBalanceData();
            try
            {
                rc.WalletAddress = wallet.WalletAddress;
                rc.WalletBalance = balance;
                rc.CompanyId = cu.CompanyId;

                cu.ForceAuthDb = false;
                using var assetManager = DataManagerFactory.GetDataManager<Asset, AssetDataManager>(cu);
                using var custManager = DataManagerFactory.GetDataManager<Customer, CustomerDataManager>(cu);
                using var claimManager = DataManagerFactory.GetDataManager<Claim, ClaimDataManager>(cu);
                using var mediaManager = DataManagerFactory.GetDataManager<Media, MediaDataManager>(cu);
                using var contractManager = Zuul.Data.DataManagerFactory.GetDataManager<ContractSetting, ContractSettingDataManager>(cu);

                var customer = GetCustomer(custManager, wallet.WalletAddress);
                // Look it up local first.
                var assets = new List<Asset>();

                try
                {
                    assets = assetManager.DefaultQuery().Where(a => a.AssetOwnerID == customer.ID && a.AssetTypeID == contract.ID).ToList();
                }
                catch (Exception ex)
                {
                    throw new WebThreeException("A blockchain error has occurred: assetManager " + ex);
                }
                if (assets.Any())
                {
                    try
                    {
                        var assetData = new List<string>();
                        foreach (var asset in assets)
                        {
                            if (null != asset)
                            {
                                var ownerOfMessage = new OwnerOfFunction() { TokenId = Convert.ToInt64(asset.AssetNumber) };
                                var queryHandler2 = web3.Eth.GetContractQueryHandler<OwnerOfFunction>();
                                var tokenOwner = queryHandler2
                                        .QueryAsync<string>(erc721TokenContractAddress, ownerOfMessage)
                                        .ConfigureAwait(false).GetAwaiter().GetResult();



                                if (!string.IsNullOrWhiteSpace(tokenOwner) && tokenOwner.Equals(wallet.WalletAddress, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var parent = new UiAsset();
                                    parent.IpfsItems = asset.IpfsItems;

                                    try
                                    {
                                        var ipfsItemList = asset.IpfsItems.ToList();

                                        parent = new UiAsset
                                        {
                                            Name = ipfsItemList.Count > 0 ? ipfsItemList[0].ItemName : "",
                                            Description = ipfsItemList.Count > 0 ? ipfsItemList[0].ItemDescription : "",
                                            Url = ipfsItemList.Count > 0 ? ipfsItemList[0].ItemImage : "",
                                            IpfsImage = ipfsItemList.Count > 0 ? ipfsItemList[0].ItemImage : "",
                                            TypeID = asset?.AssetTypeID ?? Guid.Empty,
                                            AssetID = asset?.ID ?? Guid.Empty,
                                            AssetOwnerID = asset?.AssetOwnerID ?? Guid.Empty,
                                            AssetNumber = asset?.AssetNumber ?? ""
                                        };

                                    }
                                    catch (Exception ex)
                                    {
                                        throw new WebThreeException("A blockchain error has occurred: UIASSHAT " + ex);
                                    }

                                    /*foreach (var child in asset.ChildAssets)
                                    {
                                        AssetData childData;
                                        try
                                        {
                                            childData = JsonConvert.DeserializeObject<AssetData>(child.AssetNumber);
                                        }
                                        catch
                                        {
                                            childData = new AssetData
                                            {
                                                Name = child.AssetNumber,
                                                Description = child.AssetNumber,
                                                Image = string.Empty
                                            };
                                        }
                                        parent.ChildAssets?.Add(new UiAsset
                                        {
                                            Name = childData.Name,
                                            Description = childData.Description,
                                            IpfsImage = childData.Image,
                                            Url = child.AssetData,
                                            TypeID = child?.AssetTypeID ?? Guid.Empty
                                        });
                                    }*/

                                    var claimAssets = claimManager.GetByParentAsset(asset.ID);

                                    if (claimAssets.Any())
                                    {
                                        foreach (var claimAsset in claimAssets)
                                        {
                                            parent.ClaimAssets?.Add(new UiClaim
                                            {
                                                Name = claimAsset.Name,
                                                ClaimTypeID = claimAsset.ClaimTypeID,
                                                Claimed = claimAsset.Claimed,
                                                ContractTypeID = claimAsset.ContractTypeID,
                                                ParentAssetID = claimAsset.ParentAssetID,

                                                Code = claimAsset.Code,
                                                Url = claimAsset.Url,
                                                TokenID = claimAsset.TokenID
                                            });
                                        }
                                    }

                                    var mediaAssets = mediaManager.GetByParentAsset(asset.ID);

                                    if (mediaAssets.Any())
                                    {
                                        foreach (var mediaAsset in mediaAssets)
                                        {
                                            parent.MediaAssets.Add(new UiMedia { Name = mediaAsset.Name, MediaTypeID = mediaAsset.MediaTypeID, ContractTypeID = mediaAsset.ContractTypeID, ParentAssetID = mediaAsset.ParentAssetID, Url = mediaAsset.Url, Description = mediaAsset.Description });
                                        }
                                    }

                                    var burnBMassets = assetManager.DefaultQuery().Where(a => a.AssetOwnerID == customer.ID && a.AssetTypeID == AssetTypes.BM1000).ToList();
                                    if (burnBMassets.Any())
                                    {
                                        foreach (var burnBMasset in burnBMassets)
                                        {
                                            var burnContractSetting = contractManager.DefaultQuery().First(a => a.ID == burnBMasset.AssetTypeID);
                                            parent.BurnBMAssets.Add(new UiAsset
                                            {
                                                Name = "Bill Murray 1000 Burn",
                                                Description = "Old Asset",
                                                IpfsImage = "https://ipfs.filebase.io/ipfs/QmbmLSuteV9Hay8GSWE81uSBNQTeuUPBaneF6FeKHrhVFY",
                                                TypeID = burnBMasset?.AssetTypeID ?? Guid.Empty,
                                                AssetNumber = burnBMasset?.AssetNumber ?? "",
                                                AssetOwnerID = burnBMasset?.AssetOwnerID ?? Guid.Empty,
                                                ContractAddress = burnContractSetting.ContractAddress,
                                                ContractType = burnContractSetting.ContractType
                                            });
                                        }
                                    }

                                    var burnTixAssets = assetManager.DefaultQuery().Where(a => a.AssetOwnerID == customer.ID && a.AssetTypeID == AssetTypes.MurryDrops).ToList();
                                    if (burnTixAssets.Any())
                                    {
                                        foreach (var burnTixasset in burnTixAssets)
                                        {
                                            var burnContractSetting = contractManager.DefaultQuery().First(a => a.ID == burnTixasset.AssetTypeID);
                                            parent.BurnTixAssets.Add(new UiAsset
                                            {
                                                Name = "Bill Murray 1000: Event",
                                                Description = "Placeholder Ticket",
                                                IpfsImage = "https://ipfs.filebase.io/ipfs/QmbmLSuteV9Hay8GSWE81uSBNQTeuUPBaneF6FeKHrhVFY",
                                                TypeID = burnTixasset?.AssetTypeID ?? Guid.Empty,
                                                AssetNumber = burnTixasset?.AssetNumber ?? "",
                                                AssetOwnerID = burnTixasset?.AssetOwnerID ?? Guid.Empty,
                                                ContractAddress = burnContractSetting.ContractAddress,
                                                ContractType = burnContractSetting.ContractType
                                            });
                                        }
                                    }

                                    rc.OwnedAssets.Add(parent);
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new WebThreeException("A blockchain error has occurred: JohnnyTest " + e);
                    }
                    return rc;
                }

                try
                {
                    assets = assetManager.DefaultQuery().Where(a => a.AssetTypeID == contract.ID).ToList();
                    var customers = custManager.DefaultQuery().ToList();

                    for (var i = 0; i < totalSupply; ++i)
                    {
                        var ownerOfMessage = new OwnerOfFunction() { TokenId = i };
                        var queryHandler2 = web3.Eth.GetContractQueryHandler<OwnerOfFunction>();
                        var tokenOwner = queryHandler2
                            .QueryAsync<string>(erc721TokenContractAddress, ownerOfMessage)
                            .ConfigureAwait(false).GetAwaiter().GetResult();

                        var currentCustomer = customers.FirstOrDefault(c => c.WalletAddress.Equals(tokenOwner, StringComparison.InvariantCultureIgnoreCase));
                        if (null == currentCustomer)
                            currentCustomer = GetCustomer(custManager, tokenOwner);

                        var asset = assets.FirstOrDefault(a => a.AssetNumber == i.ToString() || a.AssetNumber == $"{i:000}");
                        if (asset == null)
                        {
                            asset = assetManager.Create();
                            asset.AssetData = System.Text.Json.JsonSerializer.Serialize(new List<string> { "https://www.thechivery.com", "<<Coupon Code>>" });
                            asset.AssetNumber = i.ToString();
                            //asset.AssetOwnerID = currentCustomer.ID;
                            asset.AssetTypeID = currentCustomer.ID;
                            assetManager.Add(asset);
                        }
                        else if (asset.AssetOwnerID != currentCustomer.ID)
                        {
                            asset.AssetOwnerID = currentCustomer.ID;
                            assetManager.Update(asset);
                        }

                        if (!string.IsNullOrWhiteSpace(tokenOwner) && tokenOwner.Equals(wallet.WalletAddress, StringComparison.InvariantCultureIgnoreCase))
                        {
                            rc.OwnedAssets.Add(new UiAsset
                            {
                                Name = "",
                                Description = "",
                                Url = "",
                                TypeID = AssetTypes.MurrayCoin
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new WebThreeException("A blockchain error has occurred: KalvinTest " + ex);
                }

                throw new WebThreeException("NFT not contained in wallet!");
            }
            catch (WebThreeException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WebThreeException("A blockchain error has occurred: " + e);
            }
        }

        public Task<WalletBalanceData> GetNftAssets(WalletBalanceData current = null)
        {
            return Task.Run(() =>
            {
                if (null == current)
                    current = new WalletBalanceData();
                return current;
            });
        }

        public Task<string> GetByContractType()
        {
            return Task.Run(() =>
            {
                using var manager = DataManagerFactory.GetDataManager<SystemSetting, SystemSettingDataManager>(cu);
                var contractTypes = manager.Get(SystemSettingIDs.ContractTypes)?.SettingValue ?? String.Empty;

                var contractTypeObjects = JsonConvert.DeserializeObject<List<ContractTypeSettings>>(contractTypes, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                return JsonConvert.SerializeObject(contractTypeObjects);
            });
        }

        public class ContractTypeSettings
        {
            public Guid ContractTypeID { get; set; }
            public string Description { get; set; }
        }
    }
}
