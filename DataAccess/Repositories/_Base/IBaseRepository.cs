using Common.DataTransferObjects.CollectionPaging;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Base
{
    public interface IBaseRepository<TEntity>
        where TEntity : class
    {
        //GET Entity(s)
        Task<TEntity> GetAsync(params object[] id);
        Task<IEnumerable<TEntity>> GetAllAsync(bool changeTracking = true);

        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool changeTracking = true);
        Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool changeTracking = true);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, bool changeTracking = true);
        Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate);
        Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);

        //SEARCH Entities
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool changeTracking = true);
        Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);

        Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate, PagingParameter pagingParameter);
        Task<PagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, PagingParameter pagingParameter);
        Task<PagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, PagingParameter pagingParameter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);

        //ADD
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        //REMOVE
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        //AGREGATTED
        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
