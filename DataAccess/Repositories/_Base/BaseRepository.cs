using Common.DataTransferObjects.CollectionPaging;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        private readonly DbSet<TEntity> _entity;
        public BaseRepository(DbContext context)
        {
            _entity = context.Set<TEntity>();
        }

        //GET Entity(s)
        public async Task<TEntity> GetAsync(params object[] id)
        {
            return await _entity.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool changeTracking = true)
        {
            if (!changeTracking)
                return await _entity.AsNoTracking().ToListAsync();
            else
                return await _entity.ToListAsync();
        }

        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool changeTracking = true)
        {
            if (!changeTracking)
                return await _entity.AsNoTracking().SingleOrDefaultAsync(predicate);
            else
                return await _entity.SingleOrDefaultAsync(predicate);
        }

        public async Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            return await _entity.AsNoTracking().Where(predicate).Select(selector).SingleOrDefaultAsync();
        }


        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool changeTracking = true)
        {
            if (!changeTracking)
                return await _entity.AsNoTracking().FirstOrDefaultAsync(predicate);
            else
                return await _entity.FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, bool changeTracking = true)
        {
            if (!changeTracking)
                return await orderBy(_entity.AsNoTracking().Where(predicate)).FirstOrDefaultAsync();
            else
                return await orderBy(_entity.Where(predicate)).FirstOrDefaultAsync();
        }

        public async Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            return await _entity.AsNoTracking().Where(predicate).Select(selector).FirstOrDefaultAsync();
        }

        public async Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            return await orderBy(_entity.AsNoTracking().Where(predicate)).Select(selector).FirstOrDefaultAsync();
        }

        //SEARCH Entities
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool changeTracking = true)
        {
            if (!changeTracking)
                return await _entity.AsNoTracking().Where(predicate).ToListAsync();
            else
                return await _entity.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate)
        {
            return await _entity.AsNoTracking().Where(predicate).Select(selector).ToListAsync();
        }

        public async Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            return await orderBy(_entity.AsNoTracking().Where(predicate)).Select(selector).ToListAsync();
        }

        public async Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate, PagingParameter pagingParameter)
        {
            IQueryable<TEntity> source = _entity.AsNoTracking().Where(predicate);
            return await CalculatePagedList(pagingParameter, source);
        }

        public async Task<PagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, PagingParameter pagingParameter)
        {
            IQueryable<TResult> source = _entity.AsNoTracking().Where(predicate).Select(selector);
            return await CalculatePagedList(pagingParameter, source);
        }

        public async Task<PagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, PagingParameter pagingParameter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            IQueryable<TResult> source = orderBy(_entity.AsNoTracking().Where(predicate)).Select(selector);
            return await CalculatePagedList(pagingParameter, source);
        }

        //ADD
        public async Task AddAsync(TEntity entity)
        {
            await _entity.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _entity.AddRangeAsync(entities);
        }

        //REMOVE
        public void Remove(TEntity entity)
        {
            _entity.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _entity.RemoveRange(entities);
        }

        //AGREGATION
        public async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _entity.AsNoTracking().Where(predicate).AnyAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _entity.AsNoTracking().Where(predicate).CountAsync();
        }

        private async Task<PagedList<TResult>> CalculatePagedList<TResult>(PagingParameter pagingParameter, IQueryable<TResult> source)
        {
            int count;
            List<TResult> items;

            if (pagingParameter.PageSize != 0)
            {
                count = await source.CountAsync();
                items = await source.Skip((pagingParameter.PageNumber - 1) * pagingParameter.PageSize).Take(pagingParameter.PageSize).ToListAsync();
            }
            else
            {
                items = await source.ToListAsync();
                count = items.Count;
            }

            return new PagedList<TResult>(items, new PagingMetadata(count, pagingParameter.PageNumber, pagingParameter.PageSize));
        }
    }
}
