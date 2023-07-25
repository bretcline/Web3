using ProtonPack.Data;
using System.Net;
using System.Text;
using System.Configuration;
using static WebThree.Shared.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Zuul.BusinessLogic.Data;

namespace DataTester
{
    internal class ApiTesting
    {
        CompanyUser companyUser { get; set; }

        Encoding encoding = Encoding.ASCII;

        public HttpWebRequest GetRequest( string Url, string payload, bool companyId = true, bool userId = false, bool sessionId = false)
        {

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var req = (HttpWebRequest)WebRequest.Create(Url);
#pragma warning restore SYSLIB0014 // Type or member is obsolete

            req.Method = "POST";
            req.ContentType = "application/json";

            if (companyId)
                req.Headers.Add("CompanyId", companyUser.CompanyId.ToString());
            if (userId)
                req.Headers.Add("UserId", companyUser.AdminUserId.ToString());
            if (sessionId)
                req.Headers.Add("SessionId", companyUser.SessionId.ToString());

            var stream = req.GetRequestStream();
            byte[] buffer = Encoding.UTF8.GetBytes(payload);
            stream.Write(buffer, 0, buffer.Length);

            return req;
        }


        public CompanyUser Login( string userName, string password )
        {
            var uidPwd = new UidPwd { UserName = userName, Password = password };
            string json = WebThree.Shared.Utilities.SerializeObjectNoCache(uidPwd);

            var req = GetRequest("https://zuul-dev.azurewebsites.net/api/Auth/Login", json, false);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                companyUser = JsonConvert.DeserializeObject<CompanyUser>(responseText);
                Console.WriteLine(responseText);
            }
            return companyUser;
        }

        public Guid VerifyAndQueue( string walletAddress )
        {
            string json = $"{{\"WalletAddress\": \"{walletAddress}\" }}";

            var req = GetRequest("https://protonpack-dev.azurewebsites.net/api/Blockchain/Verify", json);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            var QueueToken = Guid.Empty;

            using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                QueueToken = JsonConvert.DeserializeObject<Guid>(responseText);
                Console.WriteLine(responseText);
            }
            return QueueToken;
        }

        public int CheckPosition( Guid QueueToken )
        {

            var json = $"{{\"QueueToken\": \"{QueueToken.ToString()}\" }}";

            var req = GetRequest("http://localhost:7071/api/Blockchain/CheckPosition", json);
            var res = (HttpWebResponse)req.GetResponse();

            var position = -1;
            using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                position = JsonConvert.DeserializeObject<int>(responseText);
                Console.WriteLine(responseText);
            }
            return position;
        }



        public string ConfirmToken(Guid QueueToken)
        {
            var json = $"{{\"QueueToken\": \"{QueueToken}\" }}";

            var req = GetRequest("http://localhost:7071/api/Blockchain/Confirm", json);
            var res = (HttpWebResponse)req.GetResponse();

            var rc = string.Empty;
            using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                rc = JsonConvert.DeserializeObject<string>(responseText);
                Console.WriteLine(responseText);
            }
            return rc;
        }
    }
}
