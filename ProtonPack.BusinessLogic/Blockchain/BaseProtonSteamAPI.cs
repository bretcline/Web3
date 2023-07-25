using Nethereum.Web3;
using Nethereum.Contracts;
using Newtonsoft.Json;
using Nethereum.Web3.Accounts;
using Nethereum.RPC.Eth.DTOs;

namespace WebThree.Shared.Blockchain
{
    public abstract class BaseProtonSteamAPI
    {
        public string Address { get; protected set; }

        public Web3 web3 { get; protected set; }

        public string ProviderURL { get; protected set; }

        public int ChainID { get; protected set; }

        public BaseProtonSteamAPI(string address, string providerURL, string chainApiKey, int chainID)
        {
            Initialize( address, providerURL, chainApiKey, chainID);
        }

        protected void Initialize(string address, string providerURL, string chainApiKey, int chainID)
        {
            Address = address;
            ProviderURL = $"{providerURL}{chainApiKey}";
            web3 = new Web3(providerURL);
            ChainID = chainID;
        }

        public async Task<T> Call<T>(string functionName, params object[] functionInput)
        {
            var contract = GenerateUnsignedContract();
            var function = contract.GetFunction(functionName);
            var result = await function.CallAsync<T>(functionInput);
            return result;
        }

        public async Task<TransactionReceipt> CallSigned(string functionName, string signer, params object[] functionInput)
        {
            Contract contract;
            Account account;
            Web3 web3;
            GenerateSignedContract(signer, out contract, out account, out web3);
            var function = contract.GetFunction(functionName);

            var timePreferenceFeeSuggesionStrategy = web3.FeeSuggestion.GetTimePreferenceFeeSuggestionStrategy();
            web3.TransactionManager.Fee1559SuggestionStrategy = timePreferenceFeeSuggesionStrategy;
            var fee = await timePreferenceFeeSuggesionStrategy.SuggestFeeAsync();

            Console.WriteLine(account.PublicKey);

            var gas = await function.EstimateGasAsync(account.Address, null, null);

            var receiptAmountSend = function.SendTransactionAndWaitForReceiptAsync(from: account.Address, gas: gas, value: null
                , maxFeePerGas: new Nethereum.Hex.HexTypes.HexBigInteger(fee.MaxFeePerGas.Value)
                , maxPriorityFeePerGas: new Nethereum.Hex.HexTypes.HexBigInteger(fee.MaxPriorityFeePerGas.Value)
                , functionInput);
            return await receiptAmountSend;
        }

        protected abstract Contract GenerateUnsignedContract();
        protected abstract void GenerateSignedContract(string signer, out Contract contract, out Account account, out Web3 web3);
    }

}
