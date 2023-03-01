using Contracts.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Contracts.Common.Interfaces
{

    #region TContext
    public interface IRepositoryQueryBase<T, K, TContext> where T : EntityBase<K>
        where TContext : DbContext
    {
        IQueryable<T> FindAll(bool trackChanges = false);
        IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        Task<T?> GetByIdAsync(K id);
        Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties);


    }

    public interface IRepositoryBaseAsync<T, K, TContext> : IRepositoryQueryBase<T, K, TContext>
        where T : EntityBase<K>
        where TContext : DbContext
    {
        Task<K> CreateAsync(T Entity);
        Task<IList<K>> CreateListAsync(IEnumerable<T> entities);
        Task UpdateAsync(T Entity); 
        Task UpdateListAsync(IEnumerable<T> entities);
        Task DeleteAsync(T Entity); 
        Task DeleteListAsync(IEnumerable<T> entities);
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task EndTransactionAsync();
        Task RollbackTransactionAsync();
    }

    #endregion

    #region No TContext
    public interface IRepositoryQueryBase<T, K> where T : EntityBase<K>
    {
        IQueryable<T> FindAll(bool trackChanges = false);
        IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        Task<T?> GetByIdAsync(K id);
        Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties);


    }

    public interface IRepositoryBaseAsync<T, K> : IRepositoryQueryBase<T, K>
        where T : EntityBase<K>
    {
        Task<K> CreateAsync(T Entity);
        Task<IList<K>> CreateListAsync(IEnumerable<T> entities);
        Task UpdateAsync(T Entity);
        Task UpdateListAsync(IEnumerable<T> entities);
        Task DeleteAsync(T Entity);
        Task DeleteListAsync(IEnumerable<T> entities);
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task EndTransactionAsync();
        Task RollbackTransactionAsync();
    }
    #endregion
}
