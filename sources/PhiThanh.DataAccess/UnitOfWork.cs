using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PhiThanh.Core;
using PhiThanh.DataAccess.Kernel;

namespace PhiThanh.DataAccess
{
    public class UnitOfWork(DbContext context) : IUnitOfWork
    {
        private readonly DbContext _context = context;
        private IDbContextTransaction _transaction;

        public DbSet<TEntity> Get<TEntity>() where TEntity : BaseEntity
        {
            return _context.Set<TEntity>();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _transaction ??= _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
        }

        public void RollBackTransaction()
        {
            _transaction.Rollback();
        }
    }
}
