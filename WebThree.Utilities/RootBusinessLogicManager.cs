using static WebThree.Shared.Utilities;

namespace WebThree.Shared.Data
{
    public abstract class RootBusinessLogicManager<T> : IBusinessLogicManager<T>, IDisposable where T : class
    {
        protected CompanyUser companyUser { get; set; }

        protected IDataManager<T> manager { get; set; }

        public RootBusinessLogicManager(CompanyUser cu, Func<CompanyUser, IDataManager<T>> dataManagerFactory)
        {
            companyUser = cu;

            manager = dataManagerFactory(cu);
        }
        public int RecordCount()
        {
            return manager.RecordCount();
        }


        public Task<T> Add(T entity)
        {
            return Task.Run(() => manager.Add(entity));
        }

        public Task<List<T>> BulkAdd(List<T> entity)
        {
            return Task.Run(() => manager.BulkAdd(entity));
        }

        public T Create()
        {
            return manager.Create();
        }

        public void Delete(Guid id)
        {
            manager.Delete(id);
        }

        public Task<T> Get(Guid id)
        {
            return Task.Run(() => manager.Get(id));
        }

        public Task<List<T>> GetAll()
        {
            return Task.Run(() => manager.GetAll());
        }

        public Task<PagedData<T>> GetPaged(int pageIndex, int pageSize)
        {
            return Task.Run(() => manager.GetPaged(pageIndex, pageSize));
        }


        public Task<T> Update(T entity, T? item = null)
        {
            return Task.Run(() => manager.Update(entity, item));
        }

        public void Dispose()
        {
            manager.Dispose();
        }
    }

}
