using Microsoft.EntityFrameworkCore.Query;
using PhiThanh.Core;
using PhiThanh.DataAccess.Entities;
using System.Linq.Expressions;

namespace PhiThanh.DataAccess.Kernel
{
    public interface IReadOnlyRepository<TEntity> : IDisposable where TEntity : BaseEntity
    {
        IQueryable<TEntity> Query(bool isTrackingEntities = false);

        PagingResult<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null!,
            PagingFilterOption filterOptions = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false);

        List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null!,
            PagingFilterOption filterOptions = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false);

        Task<PagingResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate = null!,
            PagingFilterOption filterOptions = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false);

        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null!,
            PagingFilterOption filterOptions = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false);

        TEntity? GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false);
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false);

        Task<TEntity> GetByIdAsync(Guid id, bool asTracking = false);
        TEntity GetById(Guid id, bool asTracking = false);

        bool Any(Expression<Func<TEntity, bool>> predicate = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!);

        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
