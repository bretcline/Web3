namespace WebThree.Shared.Data
{
    public interface IBusinessLogicManager<T>
    {
        Task<List<T>> GetAll();
        Task<T> Get(Guid id);
        Task<T> Add(T entity);
        Task<T> Update(T entity, T? item);
        T Create();
        void Delete(Guid id);
    }
}
