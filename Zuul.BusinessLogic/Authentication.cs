using System.Security.Cryptography;
using System.Text;
using Zuul.BusinessLogic.Data;
using Zuul.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.BusinessLogic
{
    public class Authentication
    {
        public CompanyUser Login( UidPwd uidPwd )
        {
            var user = new CompanyUser();

            try
            {
                using UserDataManager? manager = DataManagerFactory.GetDataManager<User, UserDataManager>();
                if( manager != null )
                {
                    user = manager.VerifyUser(uidPwd);
                }
            }
            catch
            {
                throw new Exception("Invalid Username/Password.");
            }

            return user;
        }

        public Guid CreateAuthToken( string payload )
        {
            var manager = DataManagerFactory.GetDataManager<Validation>();
            var authToken = manager.Create();

            authToken.ValidatedPayload = payload;
            authToken = manager.Add(authToken);

            return authToken.ID;
        }

        public bool VerifyAuthToken( Validation authToken )
        {
            var rc = false;
            if( authToken != null )
            {
                try
                {
                    var manager = DataManagerFactory.GetDataManager<Validation, ValidationDataManager>();
                    var dbToken = manager.GetUnvalidated(authToken.ID);

                    if( dbToken != null )
                    {
                        var authChecksum = CreateMD5(authToken.ValidatedPayload);
                        var dbChecksum = CreateMD5(dbToken.ValidatedPayload);

                        if( authChecksum == dbChecksum )
                        {
                            dbToken.ValidatedDate = DateTime.UtcNow;
                            dbToken.Validated = true;
                            manager.Update(dbToken);
                            rc = true;
                        }
                    }
                }
                catch
                {
                    rc = false;
                }
            }
            return rc;
        }

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}