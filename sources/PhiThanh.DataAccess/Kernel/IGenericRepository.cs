using PhiThanh.Core;

namespace PhiThanh.DataAccess.Kernel
{
    public interface IGenericRepository<TEntity> : IReadOnlyRepository<TEntity>, IEditableRepository<TEntity> where TEntity : BaseEntity
    {
    }
}
