using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PhiThanh.Core;

namespace PhiThanh.DataAccess.Kernel
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        IDbContextTransaction BeginTransaction();
        void CommitTransaction();
        public void RollBackTransaction();
        DbSet<TEntity> Get<TEntity>() where TEntity : BaseEntity;
    }
}
