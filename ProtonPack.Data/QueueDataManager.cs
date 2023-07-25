
using ProtonPack.Data;
using System.Configuration;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data
{
    public class QueueDataManager : BaseDataManager<Queue>
    {
        internal QueueDataManager(CompanyUser cu) : base(cu)
        {
        }

        public int GetQueuePosition( Guid queueID )
        {
            var list = DefaultQuery( q => !q.Verified && q.ContractTypeID == CompanyUser.ContractTypeId).OrderBy(q => q.CreatedDate).Select( q => q.ID ).ToList();

            var verifiedCount = DefaultQuery().Count(q => q.Verified && q.ContractTypeID == CompanyUser.ContractTypeId);

            var queueIndex = list.IndexOf(queueID);

            using var settings = new SystemSettingDataManager(CompanyUser);
            var count = Convert.ToInt32( settings.First(s => s.ID == SystemSettingIDs.MaxQueueCount)?.SettingValue ?? "8" );

            return (verifiedCount >= count) ? queueIndex + 1 : queueIndex;
        }

        public Guid GetTopToken()
        {
            var item = DefaultQuery().Where(q => q.ContractTypeID == CompanyUser.ContractTypeId).OrderBy(q => q.CreatedDate).First();
            return item.ID;
        }

        public Queue GetByCustomerID( Guid iD)
        {
            return First(q => q.CustomerID == iD && q.ContractTypeID == CompanyUser.ContractTypeId);
        }

        public bool CheckQueueByCustomerID(Guid iD)
        {
            var rc = Context.Queryable<Queue>().Any(q => q.CustomerID == iD && q.ContractTypeID == CompanyUser.ContractTypeId);
            return rc;
        }

        public void QueueCleanup()
        {
            var verifiedList = DefaultQuery(q => q.Verified && q.CreatedDate < DateTime.UtcNow.AddSeconds(-125)).ToList();
            foreach (var item in verifiedList)
            {
                item.Deleted = true;
                Context.Updateable(item).ExecuteCommand();
            }

            var items = DefaultQuery( q => !q.Verified ).OrderBy(q => q.CreatedDate).ToList();
            foreach (var item in items)
            {
                if (item.CreatedDate < DateTime.UtcNow.AddMinutes(-150))
                {
                    item.Deleted = true;
                    Context.Updateable(item).ExecuteCommand();
                }
            }
        }

        public void ClearAll()
        {
            /// This is the ineffecient way to do this...maybe a bulk update command?
            
            var verifiedList = DefaultQuery(q => q.ContractTypeID == CompanyUser.ContractTypeId).ToList();
            foreach (var item in verifiedList)
            {
                item.Deleted = true;
                Context.Updateable(item).ExecuteCommand();
            }
        }

        public void ClearNext()
        {
            var item = DefaultQuery(q => q.ContractTypeID == CompanyUser.ContractTypeId).OrderBy(q => q.CreatedDate).First();
            this.Delete(item.ID);
        }
    }
}
