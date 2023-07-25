namespace WebThree.Shared.Data
{
    public interface IDataManager<T> : IDisposable
    {
        List<T> GetAll();

        PagedData<T> GetPaged(int pageIndex, int pageSize);

        T Get(Guid id);
        T Add(T entity);
        T Update(T entity, T? item);
        void Delete(Guid id);
        T Create();

        int RecordCount();

        List<T> BulkAdd(List<T> entities);
    }
}
