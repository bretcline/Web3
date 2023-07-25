using ProtonPack.Data;
using System.Net;
using System.Text;
using System.Configuration;
using static WebThree.Shared.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Zuul.BusinessLogic.Data;
using DataTester;
using ProtonPack.BusinessLogic;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var encoding = ASCIIEncoding.ASCII;


var api = new ApiTesting();

var companyUser = api.Login("DevUser", "ou812");

//var tokenList = new List<Guid>();

var custtesting = new ProtonPack.BusinessLogic.CustomerManager(companyUser);
//var custlist = ( await custtesting.GetAll() ).Where( w => !string.IsNullOrWhiteSpace( w.WalletAddress) ).ToList();


//for (int i = 0; i < 10; i++)
//{
//    tokenList.Add(api.VerifyAndQueue(custlist[Random.Shared.Next(0, custlist.Count - 1)].WalletAddress));
//}

//foreach( var token in tokenList )
//    Console.WriteLine( $"{token} at {api.CheckPosition(token)}" );


var cu = new CompanyUser { CompanyId = new Guid("4C3E33FB-889A-4908-9798-FF368A29254C"), CompanyOnly = true };
var auth = new BlockchainManager(cu);
auth.WalkCryptoPunks();


#pragma warning disable SYSLIB0014 // Type or member is obsolete
var req = (HttpWebRequest)WebRequest.Create("http://localhost:7073/api/User/GetAll");
#pragma warning restore SYSLIB0014 // Type or member is obsolete

req.Method = "POST";
req.ContentType = "application/json";
req.Headers.Add("CompanyId", companyUser.CompanyId.ToString());
req.Headers.Add("UserId", companyUser.AdminUserId.ToString());
req.Headers.Add("SessionId", companyUser.SessionId.ToString());
var res = (HttpWebResponse)req.GetResponse();

using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
{
    string responseText = reader.ReadToEnd();
    Console.WriteLine(responseText);
}




var dataa = new { WalletAddress = "0xc60233FFD95d4Cb09C4F51924e8bE586Cd437F19" };

var json = JsonConvert.SerializeObject(dataa);



//req = api.GetRequest("http://localhost:7071/api/Blockchain/WalkCryptoPunks", json);
//req.Headers.Add("ContractType", ContractType.TheDreamTeam);
//req.Headers.Remove("CompanyId");
//req.Headers.Add("CompanyId", "4C3E33FB-889A-4908-9798-FF368A29254C");
//res = (HttpWebResponse)req.GetResponse();


//dataa = new { WalletAddress = "0xc60233FFD95d4Cb09C4F51924e8bE586Cd437F19" };

//json = JsonConvert.SerializeObject(dataa);

//req.Headers.Remove("CompanyId");
//req.Headers.Add("CompanyId", companyUser.CompanyId.ToString());

//req = api.GetRequest("http://localhost:7071/api/Blockchain/VerifyOwnership", json);
//req.Headers.Add("ContractType", ContractType.TheDreamTeam);
//res = (HttpWebResponse)req.GetResponse();

//dataa = new { WalletAddress = "0x51688CD36C18891167E8036bde2A8Fb10eC80C43" };

//json = JsonConvert.SerializeObject(dataa);


//req = api.GetRequest("http://localhost:7071/api/Blockchain/VerifyOwnership", json);
//req.Headers.Add("ContractType", ContractType.CryptoPunk);
//res = (HttpWebResponse)req.GetResponse();









json = "{\"Id\": \"92BAF8DA-A429-4851-AA91-051253CA3F47\" }";

req = api.GetRequest("http://localhost:7071/api/Customer/GetSummary", json);
res = (HttpWebResponse)req.GetResponse();

using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
{
    string responseText = reader.ReadToEnd();
    Console.WriteLine(responseText);
}


json = "{\"Id\": \"92BAF8DA-A429-4851-AA91-051253CA3F47\" }";

req = api.GetRequest("http://localhost:7071/api/Company/GetSummary", json);
res = (HttpWebResponse)req.GetResponse();

using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
{
    string responseText = reader.ReadToEnd();
    Console.WriteLine(responseText);
}


var data = new UserAssetData { CompanyID = companyUser.CompanyId, UserID = "0xc60233ffd95d4cb09c4f51924e8be586cd437f19", AssetID = "10002" };
json = JsonConvert.SerializeObject(data);

req = api.GetRequest("http://localhost:7071/api/Behaviors/Like", json);
res = (HttpWebResponse)req.GetResponse();





var editCust = await custtesting.Get(new Guid("AC8CF705-A1DB-4B89-8570-0436B99C536D"));
editCust.FirstName = "Pete";

json = JsonConvert.SerializeObject(editCust);

req = api.GetRequest("http://localhost:7071/api/Customer/Update", json, true, true, true);
res = (HttpWebResponse)req.GetResponse();

using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
{
    string responseText = reader.ReadToEnd();
    Console.WriteLine(responseText);

    var rule = JsonConvert.DeserializeObject<Customer>(responseText);
    Console.WriteLine(rule.FirstName);

}




var testing = new ProtonPack.BusinessLogic.EventManager(companyUser);
var list = await testing.GetAll();

foreach (var item in list)
{
    Console.WriteLine(item.EventName);
}

var cmpTesting = new ProtonPack.BusinessLogic.CompanyManager(companyUser);
var cmpList = await cmpTesting.GetAll();

foreach ( var item in cmpList)
{
    Console.WriteLine(item.CompanyName);
}


#pragma warning disable SYSLIB0014 // Type or member is obsolete
req = (HttpWebRequest)WebRequest.Create("http://localhost:7071/api/Events/Get");
#pragma warning restore SYSLIB0014 // Type or member is obsolete

req.Method = "POST";
req.ContentType = "application/json";
req.Headers.Add("CompanyId", companyUser.CompanyId.ToString());
req.Headers.Add("UserId", companyUser.AdminUserId.ToString());
req.Headers.Add("SessionId", companyUser.SessionId.ToString());
var stream = req.GetRequestStream();
json = "{\"Id\": \"A4122C3F-3768-4F2D-BF18-6901FD3CA515\" }";
var buffer = Encoding.UTF8.GetBytes(json);
stream.Write(buffer, 0, buffer.Length);
res = (HttpWebResponse)req.GetResponse();

using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
{
    string responseText = reader.ReadToEnd();
    Console.WriteLine(responseText);
}


#pragma warning disable SYSLIB0014 // Type or member is obsolete
req = (HttpWebRequest)WebRequest.Create("http://localhost:7071/api/Events/GetAll");
#pragma warning restore SYSLIB0014 // Type or member is obsolete

req.Method = "POST";
req.ContentType = "application/json";
req.Headers.Add("CompanyId", companyUser.CompanyId.ToString());
req.Headers.Add("UserId", companyUser.AdminUserId.ToString());
req.Headers.Add("SessionId", companyUser.SessionId.ToString());
res = (HttpWebResponse)req.GetResponse();

encoding = ASCIIEncoding.ASCII;
using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
{
    string responseText = reader.ReadToEnd();
    Console.WriteLine(responseText);
}

#pragma warning disable SYSLIB0014 // Type or member is obsolete
req = (HttpWebRequest)WebRequest.Create("http://localhost:7071/api/Rules/Get");
#pragma warning restore SYSLIB0014 // Type or member is obsolete

req.Method = "POST";
req.ContentType = "application/json";
req.Headers.Add("CompanyId", companyUser.CompanyId.ToString());
req.Headers.Add("UserId", companyUser.AdminUserId.ToString());
req.Headers.Add("SessionId", companyUser.SessionId.ToString());
stream = req.GetRequestStream();
json = "{\"Id\": \"255DB11E-1C2C-4873-8D7F-A3B3D1152E0F\" }";
buffer = Encoding.UTF8.GetBytes(json);
stream.Write(buffer, 0, buffer.Length);
res = (HttpWebResponse)req.GetResponse();

using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
{
    string responseText = reader.ReadToEnd();
    Console.WriteLine(responseText);

    var rule = JsonConvert.DeserializeObject<Rule>(responseText);
    Console.WriteLine(rule.Parameters[0].ParameterName);

}


Console.ReadKey();

