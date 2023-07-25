using WebThree.Shared.Data;
using Zuul.BusinessLogic.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.Data
{
    public class UserDataManager : BaseDataManager<User>
    {
        internal UserDataManager(CompanyUser? cu) : base(cu)
        {
        }

        public CompanyUser VerifyUser( UidPwd uidPwd )
        {
            CompanyUser rc = null;
            try
            {
                /*if (uidPwd.IsEncrypted)
                {
                    uidPwd.Password = uidPwd.DecryptStringAES(uidPwd.Password);
                }*/

                User user = null;
                if (!string.IsNullOrWhiteSpace(uidPwd.WalletAddress))
                {
                    user = First(i => i.WalletAddress.Equals(uidPwd.WalletAddress, StringComparison.CurrentCultureIgnoreCase));
                }
                /*else
                {
                    user = First(i => i.UserName.Equals(uidPwd.UserName, StringComparison.CurrentCultureIgnoreCase));
                    var pwd = uidPwd.Password;
                    if (user.Encrypted)
                        pwd = uidPwd.DecryptStringAES(user.AccessKey);
                    if (pwd != uidPwd.Password)
                    {
                        user = null;
                    }
                }*/

                if( null != user )
                {
                    CompanyUser.AdminUserId = user.ID;
                    CompanyUser.UserName = user.UserName;
                    CompanyUser.CompanyId = user.CompanyID;
                    CompanyUser.ForceAuthDb = true;

                    using var manager = DataManagerFactory.GetDataManager<UserSession>(CompanyUser);
                    
                    var userSession = manager.Create();
                    userSession.UserID = user.ID;
                    userSession.StartDate = userSession.LastUpdated = DateTime.UtcNow;
                    
                    userSession = manager.Add( userSession );

                    rc = new CompanyUser
                    {
                        UserName = user.UserName,
                        AdminUserId = user.ID,
                        CompanyId = user.CompanyID,
                        SessionId = userSession.ID
                    };
                }
            }
            catch
            {
                throw new Exception("Invalid Username/Password.");
            }
            if( null == rc )
            {
                throw new Exception("Invalid Username/Password.");
            }
            return rc;
        }
    }
}