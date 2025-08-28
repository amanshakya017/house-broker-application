using HouseBrokerApp.Core.Interfaces;

namespace HouseBrokerApp.Infrastructure.Repository
{
    /// <summary>
    /// Implements the Unit of Work pattern to ensure that a group of 
    /// database operations can be executed as a single transaction.
    /// This class also manages repositories dynamically using caching.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a repository for the specified entity type.
        /// If it doesn't exist, creates a new GenericRepository for that type.
        /// </summary>
        /// <typeparam name="T">Entity type (class).</typeparam>
        /// <returns>An instance of <see cref="IRepository{T}"/> for the entity.</returns>
        public IRepository<T> Repository<T>() where T : class
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                var repository = new GenericRepository<T>(_context);
                _repositories[typeof(T)] = repository;
            }
            return (IRepository<T>)_repositories[typeof(T)];
        }

        /// <summary>
        /// Persists all changes made in the current transaction to the database.
        /// </summary>
        /// <returns>The number of affected rows.</returns>
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
