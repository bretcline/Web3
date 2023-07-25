using SqlSugar;
using static WebThree.Shared.Utilities;

namespace WebThree.Shared.Data
{
    public abstract class RootDataManager : IDisposable
    {
        readonly internal SqlSugarClient context;
        private bool disposedValue;
        public CompanyUser CompanyUser { get; set; }

        public RootDataManager( RootDataManager db )
        {
            context = db.context;
            CompanyUser = db.CompanyUser;
            disposedValue = false;
        }

        public RootDataManager(CompanyUser? cu)
        {
            disposedValue = false;
            CompanyUser = cu ?? new CompanyUser();

            var connString = Utilities.GetSqlAzureConnectionString( CompanyUser );

            context = new SqlSugarClient(new List<ConnectionConfig>()
            {
                new ConnectionConfig(){ ConfigId="0", DbType=SqlSugar.DbType.SqlServer,  ConnectionString=connString, IsAutoCloseConnection = true },
            });

            //single table query gobal filter
            context.QueryFilter.Add(new SqlFilterItem()
            {
                FilterValue = filterDb =>
                {
                    //Writable logic
                    return new SqlFilterResult() { Sql = " Deleted = 0" };//Global string perform best
                }
            });

            //Multi-table query gobal filter
            context.QueryFilter.Add(new SqlFilterItem()
            {
                FilterValue = filterDb =>
                {
                    //Writable logic
                    return new SqlFilterResult() { Sql = " main.Deleted = 0" };
                },
                IsJoinQuery = true
            });

            context.Open();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    context.Close();
                    context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseDataManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
