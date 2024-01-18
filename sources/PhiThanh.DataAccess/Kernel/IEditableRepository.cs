using PhiThanh.Core;
using System.Linq.Expressions;

namespace PhiThanh.DataAccess.Kernel
{
    public interface IEditableRepository<TEntity> : IDisposable where TEntity : BaseEntity
    {
        TEntity Insert(TEntity entity, bool saveChange = false);
        int Insert(IEnumerable<TEntity> entities, bool saveChange = false);
        int InsertOrUpdate(IEnumerable<TEntity> entities, bool saveChange = false);
        TEntity InsertOrUpdate(TEntity entity, bool saveChange = false);
        Task<TEntity> InsertAsync(TEntity entity, bool saveChange = false);
        Task<int> InsertAsync(IEnumerable<TEntity> entities, bool saveChange = false);
        Task<int> InsertOrUpdateAsync(IEnumerable<TEntity> entities, bool saveChange = false);
        Task<TEntity> InsertOrUpdateAsync(TEntity entity, bool saveChange = false);
        TEntity Update(TEntity entity, bool saveChange = false);
        int Update(IEnumerable<TEntity> entities, bool saveChange = false);
        Task<TEntity> UpdateAsync(TEntity entity, bool saveChange = false);
        Task<int> UpdateAsync(IEnumerable<TEntity> entities, bool saveChange = false);
        int Delete(Guid id, bool saveChange = false);
        int Delete(TEntity entity, bool saveChange = false);
        int Delete(IEnumerable<TEntity> entities, bool saveChange = false);
        int Delete(Expression<Func<TEntity, bool>> predicate, bool saveChange = false);
        Task<int> DeleteAsync(Guid id, bool saveChange = false);
        Task<int> DeleteAsync(TEntity entity, bool saveChange = false);
        Task<int> DeleteAsync(IEnumerable<TEntity> entities, bool saveChange = false);
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool saveChange = false);
        int SoftDelete(TEntity entity, bool saveChange = false);
        int SoftDelete(IEnumerable<TEntity> entities, bool saveChange = false);
        int SoftDelete(Expression<Func<TEntity, bool>> predicate, bool saveChange = false);
        Task<int> SoftDeleteAsync(TEntity entity, bool saveChange = false);
        Task<int> SoftDeleteAsync(IEnumerable<TEntity> entities, bool saveChange = false);
        Task<int> SoftDeleteAsync(Expression<Func<TEntity, bool>> predicate, bool saveChange = false);
    }
}
