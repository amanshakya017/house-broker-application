using HouseBrokerApp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HouseBrokerApp.Infrastructure.Repository
{
    /// <summary>
    /// A generic repository implementation for performing CRUD operations 
    /// on entities in the database using Entity Framework Core.
    /// This follows the Repository Pattern and abstracts data access logic.
    /// </summary>
    /// <typeparam name="T">Entity type (must be a class).</typeparam>
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Retrieves all entities of type T from the database.
        /// </summary>
        /// <returns>A list of entities.</returns>
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        /// <summary>
        /// Retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the entity.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        /// <summary>
        /// Finds entities matching the provided condition (predicate).
        /// </summary>
        /// <param name="predicate">Lambda expression filter (e.g., x => x.Name == "Test").</param>
        /// <returns>A filtered list of entities.</returns>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        /// <summary>
        /// Adds a new entity to the database asynchronously.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        public void Update(T entity) => _dbSet.Update(entity);

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}
