using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Crypto.Generators;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using WebThree.Shared.Data;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using JWT;
using static WebThree.Shared.Utilities;
using System.Security.Claims;

namespace WebThree.Shared
{
    public static class AssetTypes
    {
        public static readonly Guid TheDreamTeam =  new("00000001-0000-0000-0000-000000000001");
        public static readonly Guid BAYC =          new("00000002-0000-0000-0000-000000000002");
        public static readonly Guid CryptoPunk =    new("00000003-0000-0000-0000-000000000003");
        public static readonly Guid BM1000 =        new("00000004-0000-0000-0000-000000000004");
        public static readonly Guid MurryDrops =    new("00000004-0000-0000-0000-000000000005");

        // [Client+Project]-[ContractType]-[Chain]-[ContractMinterType]-[Project]
        public static readonly Guid PVP =                   new("10000000-0001-0001-0002-000000000001");
        public static readonly Guid BMOriginals =           new("40000001-0001-0001-0002-000000000001");
        public static readonly Guid BMDestinations =        new("40000001-0001-0001-0002-000000000002");
        public static readonly Guid BM3D =                  new("40000001-0001-0001-0002-000000000003");
        public static readonly Guid BMEvents =              new("40000001-0004-0001-0002-000001000000");
        public static readonly Guid BMNecessities =         new("40000001-0004-0001-0002-000002000000");

        public static readonly Guid MurrayCoin =            new("40000001-0000-0000-0000-000000000001");
        public static readonly Guid DigitalAssets =         new("40000001-0000-0000-0000-000000000002");
        public static readonly Guid DigitalAssets_Video =   new("40000001-0000-0000-0000-000000000003");
        public static readonly Guid PhysicalEvents =        new("40000001-0000-0000-0000-000000000004");

    }

    public enum ContractMinterType
    {
        WebThree = 1,
        Hypermint = 2
    }

    public enum ErcType
    {
        Erc721 = 1,
        Erc20 = 2,
        CryptoPunk = 3,
        Erc1155 = 4
    }

    public static class ListExtensions
    {
        public static IEnumerable<List<T>> SplitList<T>( this List<T> bigList, int nSize = 3)
        {
            for (int i = 0; i < bigList.Count; i += nSize)
            {
                yield return bigList.GetRange(i, Math.Min(nSize, bigList.Count - i));
            }
        }
    }


    public class GenerateJWTToken
    {
        private readonly IJwtAlgorithm _algorithm;
        private readonly IJsonSerializer _serializer;
        private readonly IBase64UrlEncoder _base64Encoder;
        private readonly IJwtEncoder _jwtEncoder;
        public GenerateJWTToken()
        {
            // JWT specific initialization.
            //_algorithm = new RSA();
            //_serializer = new JsonNetSerializer();
            //_base64Encoder = new JwtBase64UrlEncoder();
            //_jwtEncoder = new JwtEncoder(_algorithm, _serializer, _base64Encoder);
        }

        public string IssuingJWT(CompanyUser user)
        {
            var key = Utilities.GetConfigItem( "JWT_Key" );
            string token = _jwtEncoder.Encode(user, key); // Put this key in config
            return token;
        }
    }

    public class ValidateJWT
    {
        public bool IsValid { get; }
        public CompanyUser cu { get; }
        public string Role { get; }

        public ValidateJWT(HttpRequest request)
        {
            // Check if we have a header.
            if (!request.Headers.ContainsKey("Authorization"))
            {
                IsValid = false;

                return;
            }

            string authorizationHeader = request.Headers["Authorization"];

            // Check if the value is empty.
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                IsValid = false;

                return;
            }

            // Check if we can decode the header.
            IDictionary<string, object> claims = null;

            try
            {
                if (authorizationHeader.StartsWith("Bearer"))
                {
                    authorizationHeader = authorizationHeader.Substring(7);
                }

                var key = Utilities.GetConfigItem("JWT_Key");

                // Validate the token and decode the claims.
                claims = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(key)
                    .MustVerifySignature()
                    .Decode<IDictionary<string, object>>(authorizationHeader);
            }
            catch
            {
                IsValid = false;

                return;
            }

            // Check if we have user claim.
            if (!claims.ContainsKey("username"))
            {
                IsValid = false;

                return;
            }

            IsValid = true;
            cu = (CompanyUser)claims["username"];
        }
    }


    public class Utilities
    {


        public class CompanyUser
        {
            public CompanyUser() { }
            public CompanyUser(CompanyUser cu) 
            {
                ForceAuthDb = cu.ForceAuthDb;
                CompanyId = cu.CompanyId; 
                SessionId = cu.SessionId;
                UserName = cu.UserName;
                CompanyOnly = cu.CompanyOnly;
                AdminUserId = cu.AdminUserId;
                ContractTypeId = cu.ContractTypeId;
            }

            public void FromHeaders(IHeaderDictionary headers, bool throwHeaderErrors)
            {
                if (!headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                if (!headers.TryGetValue("SessionId", out var sessionIdString) && throwHeaderErrors)
                {
                    throw new MissingSessionException();
                }
                if (string.IsNullOrWhiteSpace( sessionIdString ) && !headers.TryGetValue("UserId", out var userIdString) && throwHeaderErrors)
                {
                    throw new MissingUserIdException();
                }
                if (!headers.TryGetValue("ContractType", out var contractTypeIDString) && throwHeaderErrors)
                {
                    // TODO: need to figure out if/when we need to check for this.
                    //throw new MissingUserIdException();
                }

                var companyId = GetGuid(companyIdString);
                var adminUserId = GetGuid(userIdString);
                var sessionId = GetGuid(sessionIdString);
                var contractTypeID = GetGuid(contractTypeIDString);
                var userId = string.Empty;

                if (adminUserId == Guid.Empty)
                    userId = userIdString;

                CompanyId = companyId;
                AdminUserId = adminUserId;
                UserName = userId;
                SessionId = sessionId;
                ContractTypeId = contractTypeID;

            }
            public bool CompanyOnly { get; set; }
            public Guid CompanyId { get; set; }
            public Guid AdminUserId { get; set; }
            public string? UserName { get; set; }
            public Guid SessionId { get; set; }
            public Guid ContractTypeId { get; set; }
            public string Token { get; set; }

            public bool ForceAuthDb { get; set; }  
        }


        public class WebThreeException : Exception
        {
            public WebThreeException() : base() { }
            public WebThreeException(string message) : base(message) { } 
        }

        public class MissingCompanyIdException : WebThreeException
        {
            public MissingCompanyIdException() : base("Missing CompanyID.") { }
        }

        public class MissingUserIdException : WebThreeException
        {
            public MissingUserIdException() : base("Missing UserID.") { }
        }


        public class MissingSessionException : WebThreeException
        {
            public MissingSessionException() : base("Missing Session.") { }
        }

        public class MissingContractTypeException : WebThreeException
        {
            public MissingContractTypeException() : base("Missing Contract Type.") { }
        }

        public class InvalidSessionException : WebThreeException
        {
            public InvalidSessionException() : base("Invalid Session.  Please log in again.") { }
        }

        public static Guid GetGuid(object raw)
        {
            Guid rc = Guid.Empty;
            try
            {
                if (null != raw)
                {
                    var rawGuid = raw?.ToString();
                    if (raw is string[] strings)
                    {
                        rawGuid = strings[0];
                    }
                    if (!string.IsNullOrWhiteSpace(rawGuid))
                    {
                        rc = new Guid(rawGuid.Replace("\"", "").Replace("{", "").Replace("}", ""));
                    }
                }
            }
            catch
            {
                rc = Guid.Empty;
            }
            return rc;
        }


        protected static async Task<IActionResult> ValidateSessionAsync(HttpRequest req
            , bool throwHeaderErrors
            , Func<CompanyUser, string, Task<IActionResult>> code
            , Func<CompanyUser, Exception, bool> logError
            , bool logCall = true
            , [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                try
                {
                    //var auth = new ValidateJWT(req);
                    //if (!auth.IsValid)
                    //{
                    //    return new UnauthorizedResult(); // No authentication info.
                    //}

                    var companyUser = new CompanyUser();
                    companyUser.FromHeaders(req.Headers, throwHeaderErrors);

                    if( throwHeaderErrors )
                    {
                        ValidateSession(companyUser);
                    }

                    var logActionId = Guid.NewGuid();
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                    if (logCall)
                    {
                        // Log Data here;
                    }

                    var response = code(companyUser, requestBody).GetAwaiter().GetResult();

                    if (logCall)
                    {
                        // Log Data here;
                    }
                    return response;
                }
                catch
                {
                    throw;
                }
            }
            catch (MissingUserIdException me)
            {
                return new BadRequestObjectResult(me.Message);
            }
            catch (System.AggregateException e)
            {
                if (e.InnerExceptions[0] is MissingUserIdException)
                {
                    return new UnauthorizedResult();
                }
                var builder = new StringBuilder();
                foreach (var exception in e.InnerExceptions)
                {
                    if (exception is WebThreeException)
                        builder.Append($"{exception.Message}{Environment.NewLine}");
                    else
                    {
#if DEBUG
                        builder.Append($"DEBUG UNHANDLED ERROR - {exception.Message}{Environment.NewLine}");
#else
                        builder.Append($"An error has occurred, please contact WebThree support.");
#endif
                    }
                }
                return new BadRequestObjectResult(builder.ToString());
            }
            catch (WebThreeException e)
            {
                return new BadRequestObjectResult($"{e.Message}");
            }
            catch (Exception e)
            {
#if DEBUG
                return new BadRequestObjectResult($"DEBUG UNHANDLED ERROR - {e.Message}");
#else
                return new BadRequestObjectResult($"An error has occurred, please contact WebThree support.");
#endif
            }
        }

        private static void ValidateSession(CompanyUser cu)
        {
            var adminUser = new CompanyUser(cu)
            {
                ForceAuthDb = true
            };
            using var conn = new SqlConnection(GetSqlAzureConnectionString(adminUser));
            conn.Open();
            try
            {
                using var command = conn.CreateCommand();
                command.CommandText = $"SELECT COUNT(1) FROM UserSessions WHERE UserSessionID = '{adminUser.SessionId}' AND EndDate IS NULL";
                var sessionCount = (int)command.ExecuteScalar();
                if (sessionCount > 0)
                {
                    command.CommandText = $"UPDATE UserSessions SET LastUpdated = GETUTCDATE() WHERE UserSessionID = '{adminUser.SessionId}' AND EndDate IS NULL";
                    command.ExecuteNonQuery();
                }
                else
                    throw new WebThreeException("Invalid Session.");

            }
            finally
            {
                conn.Close();
            }
        }

        public static Task<IActionResult> ValidateSession<OElement>(HttpRequest req
            , Func<CompanyUser, dynamic, string, OElement> code
            , Func<CompanyUser, Exception, bool> logError
            , bool throwHeaderErrors = true
            , bool logCall = true
            , [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            return ValidateSessionAsync(req, throwHeaderErrors, async (companyUser, requestBody) =>
            {
                dynamic data = JsonConvert.DeserializeObject(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                var result = await code(companyUser, data, requestBody);

                return new OkObjectResult(SerializeObjectNoCache(result));
            }, logError, logCall);
        }

        public static Task<IActionResult> ValidateSession<TElement, OElement>(HttpRequest req
            , Func<CompanyUser, TElement, OElement> code
            , Func<CompanyUser, Exception, bool> logError
            , bool throwHeaderErrors = true
            , bool logCall = true
            , [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            return ValidateSessionAsync(req, throwHeaderErrors, async (companyUser, requestBody) =>
            {
                var data = JsonConvert.DeserializeObject<TElement>(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                dynamic result = code(companyUser, data);

                if( result is IAsyncResult )
                    return new OkObjectResult(SerializeObjectNoCache(result.Result));
                else
                    return new OkObjectResult(SerializeObjectNoCache(result));

            }, logError, logCall);
        }

        public static string SerializeObjectNoCache(object obj, Formatting format = Formatting.None, JsonSerializerSettings? settings = null)
        {
            settings ??= new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            bool reset = (settings.ContractResolver == null);
            // To reduce memory footprint, do not cache contract information in the global contract resolver.
            if (reset)
                settings.ContractResolver = new DefaultContractResolver();
            try
            {
                return JsonConvert.SerializeObject(obj, format, settings);
            }
            finally
            {
                if (reset)
                    settings.ContractResolver = null;
            }
        }



        public static bool LogError(ILogger logger, CompanyUser companyUser, Exception error, HttpRequest request)
        {
            var rc = false;
            try
            {
                if (error is AggregateException errors)
                {
                    foreach (var innerException in errors.InnerExceptions)
                    {
                        logger.LogError(innerException.Message);
                    }
                }
                else
                {
                    logger.LogError(error.Message);
                }

                rc = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return rc;
        }


        //public static string GetSqlAzureConnectionString(string name)
        //{
        //    var value = System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        //    if (string.IsNullOrWhiteSpace(value)) // Azure Functions App Service naming convention
        //    {
        //        var root = Directory.GetCurrentDirectory();
        //        var config = new ConfigurationBuilder().SetBasePath(root)
        //            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        //            .Build();
        //        conStr = config.GetSection("Values")..GetConnectionString(name);
        //        if (string.IsNullOrWhiteSpace(conStr))
        //        {
        //            conStr = config[name];
        //        }
        //    }
        //    if (string.IsNullOrWhiteSpace(conStr)) // Azure Functions App Service naming convention
        //    {
        //        conStr = System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString;
        //    }

        //    return value;
        //}



        public static string GetSqlAzureConnectionString(CompanyUser cu)// Guid companyId, bool ForceAuthDb = false )
        {
            var name = "ProtonPack";
            var conStr = System.Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"SQLAZURECONNSTR_{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(conStr)) // Azure Functions App Service naming convention
            {
                conStr = System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            if (string.IsNullOrWhiteSpace(conStr)) // Azure Functions App Service naming convention
            {
                var root = Directory.GetCurrentDirectory();
                var config = new ConfigurationBuilder().SetBasePath(root)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .Build();
                conStr = config.GetConnectionString(name);
                if (string.IsNullOrWhiteSpace(conStr))
                {
                    conStr = config[name];
                }
            }
            if (cu.CompanyId != Guid.Empty && !cu.ForceAuthDb)
            {
                using var authConnection = new SqlConnection(conStr);
                authConnection.Open();
                using var authCommand = new SqlCommand($"SELECT AccessKey from Companies WHERE CompanyID = '{cu.CompanyId}'", authConnection);
                conStr = (string)authCommand.ExecuteScalar();
                if (string.IsNullOrWhiteSpace(conStr))
                {
                    throw new WebThreeException($"No connection string for company {cu.CompanyId}.");
                }
            }

            return conStr;
        }

        public static string GetConfigItem(string name)
        {
            var value = System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(value)) // Azure Functions App Service naming convention
            {
                value = System.Configuration.ConfigurationManager.AppSettings[name];
            }
            if (string.IsNullOrWhiteSpace(value)) // Azure Functions App Service naming convention
            {
                var root = Directory.GetCurrentDirectory();
                var config = new ConfigurationBuilder().SetBasePath(root)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .Build();
                value = config.GetSection("Values")[name];
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = config[name];
                }
            }

            return value;
        }
    }

    public class AesEncrypt
    {
        public static string Encrypt(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                //var password = BCrypt.Net.BCrypt.HashPassword(key).Substring(0, 32);
                var password = key.Substring(0, 32);

                var aesKey = Encoding.UTF8.GetBytes(password);
                aes.Key = aesKey;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new())
                {
                    using (CryptoStream cryptoStream = new ((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new ((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string Decrypt(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                //var password = BCrypt.Net.BCrypt.HashPassword(key).Substring(0, 32);
                var password = key.Substring(0, 32);

                var aesKey = Encoding.UTF8.GetBytes(password);
                aes.Key = aesKey;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new(buffer))
                {
                    using (CryptoStream cryptoStream = new ((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new ((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
