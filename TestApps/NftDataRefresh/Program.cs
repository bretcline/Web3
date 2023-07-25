// See https://aka.ms/new-console-template for more information
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Ocsp;
using ProtonPack.BusinessLogic;
using System.Security.Policy;
using WebThree.Shared;
using Zuul.Data;
using static WebThree.Shared.Utilities;


internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var contractType = AssetTypes.MurryDrops;
        var companyId = new Guid("c8494fd1-d34a-461f-a176-b79bb578c17f");

        var cu = new CompanyUser { CompanyId = companyId, ContractTypeId = contractType, CompanyOnly = true };

        var contract = Zuul.Data.DataManagerFactory.GetDataManager<ContractSetting>(cu);
        var contractSettings = contract.Get( contractType );



        //var encKey = "0xC843A8fcaa6540e895798C743a51C8b3c3b3Df40";
        //var encrypted = WebThree.Shared.AesEncrypt.Encrypt(encKey, "B4_8Q~x_mSS7Oa6SuFMWWgP423UaVnC3AvCt4bNUß0xC843A8fcaa6540e895798C743a51C8b3c3b3Df40");



        /*var decrypted = WebThree.Shared.AesEncrypt.Decrypt( contractSettings.ContractAddress, contractSettings.MintKey );

        Console.WriteLine(decrypted);

        var keys = decrypted.Split( 'ß' );

        try
        {
            //6e49af8e-695c-491c-b2b2-e2e725251c5f
            //
            var creds = new ClientSecretCredential("2aa947c1-8725-4739-961d-7ce77f2f9889", "65a5a749-7422-4f7c-a079-6f44212020aa", keys[0]);// "43780ffc-19d4-4b8d-8692-d7a3385ccd67");
            var client = new SecretClient(new Uri("https://ghosttraps.vault.azure.net/"), creds);

            KeyVaultSecret secret = client.GetSecret("TestValue");

            string secretValue = secret.Value;
            Console.WriteLine($"Your secret is '{secret.Value}'.");
        }
        catch
        {

        }*/


        //try
        //{
        //    const string secretName = "TestValue";
        //    var keyVaultName = "GhostTraps";
        //    var kvUri = $"https://{keyVaultName}.vault.azure.net";

        //    var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

        //    Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
        //    var secret = await client.GetSecretAsync(secretName);
        //    Console.WriteLine($"Your secret is '{secret.Value.Value}'.");

        //}
        //catch
        //{

        //}

        //var contractTypeId = AssetTypes.BM1000;
        //var contractTypeId = AssetTypes.BMOriginals;
        List<Guid> contracts = new List<Guid>();
        contracts.Add(AssetTypes.BM1000);
        contracts.Add(AssetTypes.MurryDrops);
        contracts.Add(AssetTypes.BMOriginals);
        contracts.Add(AssetTypes.BMDestinations);
        contracts.Add(AssetTypes.PVP);
        contracts.Add(AssetTypes.BM3D);
        
        contracts.ForEach(contract => { 
            cu.ContractTypeId = contract;
            var auth = new BlockchainManager(cu);
            auth.WalkContract(contract);
        });
    }
}