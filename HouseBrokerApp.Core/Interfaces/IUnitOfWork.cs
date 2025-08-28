namespace HouseBrokerApp.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}
