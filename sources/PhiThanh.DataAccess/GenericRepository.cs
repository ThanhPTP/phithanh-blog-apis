using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.Core.Utils;
using PhiThanh.DataAccess.Kernel;
using System.Linq.Expressions;

namespace PhiThanh.DataAccess
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>()!;
        }

        public IQueryable<TEntity> Query(bool isTrackingEntities = false)
        {
            if (isTrackingEntities)
            {
                return _dbSet.AsTracking().Where(w => !w.IsDeleted);
            }
            return _dbSet.AsNoTracking().Where(w => !w.IsDeleted);
        }

        public TEntity InsertOrUpdate(TEntity entity, bool saveChange = false)
        {
            if (entity.Id == Guid.Empty)
            {
                Insert(entity, false);
            }
            else
            {
                Update(entity, false);
            }

            if (saveChange)
            {
                _context.SaveChanges();
            }

            return entity;
        }

        public int InsertOrUpdate(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            foreach (var entity in entities)
            {
                if (entity.Id == Guid.Empty)
                {
                    Insert(entity, false);
                }
                else
                {
                    Update(entity, false);
                }
            }

            if (saveChange)
            {
                return _context.SaveChanges();
            }
            return 0;
        }

        public async Task<TEntity> InsertOrUpdateAsync(TEntity entity, bool saveChange = false)
        {
            if (entity.Id == Guid.Empty)
            {
                await InsertAsync(entity, false);
            }
            else
            {
                await UpdateAsync(entity, false);
            }

            if (saveChange)
            {
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public async Task<int> InsertOrUpdateAsync(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            foreach (var entity in entities)
            {
                if (entity.Id == Guid.Empty)
                {
                    await InsertAsync(entity, false);
                }
                else
                {
                    await UpdateAsync(entity, false);
                }
            }

            if (saveChange)
            {
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public PagingResult<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null!,
            PagingFilterOption filterOptions = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false)
        {
            IQueryable<TEntity> query = Query(asTracking);
            var result = new PagingResult<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            result.TotalRecords = query.Count(w => !w.IsDeleted);

            if (filterOptions != null)
            {
                foreach (var item in filterOptions.Sorter)
                {
                    if (item.Value == "descend"
                        || item.Value == "desc")
                    {
                        query = query.OrderByDescending(item.Key.CapitalizeFirstLetter());
                    }
                    else if (item.Value == "ascend"
                        || item.Value == "asc")
                    {
                        query = query.OrderBy(item.Key.CapitalizeFirstLetter());
                    }
                }

                filterOptions.PageSize = filterOptions.PageSize <= 0 ? 10 : filterOptions.PageSize;
                filterOptions.PageIndex = filterOptions.PageIndex <= 0 ? 1 : filterOptions.PageIndex;

                query = query.Skip((filterOptions.PageIndex - 1) * filterOptions.PageSize).Take(filterOptions.PageSize);

                result.PageSize = filterOptions.PageSize;
                result.PageIndex = filterOptions.PageIndex;
            }

            result.Data = query.ToList();
            return result;
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null!,
            PagingFilterOption filterOptions = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false)
        {
            IQueryable<TEntity> query = Query(asTracking);
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (filterOptions != null)
            {
                foreach (var item in filterOptions.Sorter)
                {
                    if (item.Value == "descend"
                        || item.Value == "desc")
                    {
                        query = query.OrderByDescending(item.Key.CapitalizeFirstLetter());
                    }
                    else if (item.Value == "ascend"
                        || item.Value == "asc")
                    {
                        query = query.OrderBy(item.Key.CapitalizeFirstLetter());
                    }
                }

                filterOptions.PageSize = filterOptions.PageSize <= 0 ? 10 : filterOptions.PageSize;
                filterOptions.PageIndex = filterOptions.PageIndex <= 0 ? 1 : filterOptions.PageIndex;

                query = query.Skip((filterOptions.PageIndex - 1) * filterOptions.PageSize).Take(filterOptions.PageSize);
            }

            return [.. query];
        }

        public async Task<PagingResult<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate = null!,
            PagingFilterOption filterOptions = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false)
        {
            IQueryable<TEntity> query = Query(asTracking);
            var result = new PagingResult<TEntity>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            result.TotalRecords = query.Count(w => !w.IsDeleted);

            if (filterOptions != null)
            {
                foreach (var item in filterOptions.Sorter)
                {
                    if (item.Value == "descend"
                        || item.Value == "desc")
                    {
                        query = query.OrderByDescending(item.Key.CapitalizeFirstLetter());
                    }
                    else if (item.Value == "ascend"
                        || item.Value == "asc")
                    {
                        query = query.OrderBy(item.Key.CapitalizeFirstLetter());
                    }
                }

                filterOptions.PageSize = filterOptions.PageSize <= 0 ? 10 : filterOptions.PageSize;
                filterOptions.PageIndex = filterOptions.PageIndex <= 0 ? 1 : filterOptions.PageIndex;

                query = query.Skip((filterOptions.PageIndex - 1) * filterOptions.PageSize).Take(filterOptions.PageSize);

                result.PageSize = filterOptions.PageSize;
                result.PageIndex = filterOptions.PageIndex;
            }

            result.Data = await query.ToListAsync();
            return result;
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null!,
            PagingFilterOption filterOptions = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false)
        {
            IQueryable<TEntity> query = Query(asTracking);
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (filterOptions != null)
            {
                foreach (var item in filterOptions.Sorter)
                {
                    if (item.Value == "descend"
                        || item.Value == "desc")
                    {
                        query = query.OrderByDescending(item.Key.CapitalizeFirstLetter());
                    }
                    else if (item.Value == "ascend"
                        || item.Value == "asc")
                    {
                        query = query.OrderBy(item.Key.CapitalizeFirstLetter());
                    }
                }

                filterOptions.PageSize = filterOptions.PageSize <= 0 ? 10 : filterOptions.PageSize;
                filterOptions.PageIndex = filterOptions.PageIndex <= 0 ? 1 : filterOptions.PageIndex;

                query = query.Skip((filterOptions.PageIndex - 1) * filterOptions.PageSize).Take(filterOptions.PageSize);
            }

            return await query.ToListAsync();
        }

        public TEntity GetById(Guid id, bool asTracking = false)
        {
            if (asTracking)
            {
                return _dbSet.AsTracking().First(w => !w.IsDeleted && w.Id == id);
            }
            else
            {
                return _dbSet.AsTracking().First(w => !w.IsDeleted && w.Id == id);
            }
        }

        public async Task<TEntity> GetByIdAsync(Guid id, bool asTracking = false)
        {
            if (asTracking)
            {
                return await _dbSet.AsTracking().FirstAsync(w => !w.IsDeleted && w.Id == id);
            }
            else
            {
                return await _dbSet.AsNoTracking().FirstAsync(w => !w.IsDeleted && w.Id == id);
            }
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TEntity? GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false)
        {
            IQueryable<TEntity> query;
            if (!asTracking)
            {
                query = _dbSet.AsNoTracking().Where(w => !w.IsDeleted);
            }
            else
            {
                query = _dbSet.AsTracking().Where(w => !w.IsDeleted);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            return query.FirstOrDefault();
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!, bool asTracking = false)
        {
            IQueryable<TEntity> query;
            if (!asTracking)
            {
                query = _dbSet.AsNoTracking().Where(w => !w.IsDeleted);
            }
            else
            {
                query = _dbSet.AsTracking().Where(w => !w.IsDeleted);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync();
        }

        public TEntity Insert(TEntity entity, bool saveChange = false)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;
            SetCreatedByForEntity(entity);
            _dbSet.Add(entity);
            if (saveChange)
            {
                _context.SaveChanges();
            }
            return entity;
        }

        public int Insert(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            foreach (var entity in entities)
            {
                Insert(entity, false);
            }

            return saveChange ? _context.SaveChanges() : 0;
        }

        public async Task<TEntity> InsertAsync(TEntity entity, bool saveChange = false)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;
            SetCreatedByForEntity(entity);
            await _dbSet.AddAsync(entity);
            if (saveChange)
            {
                await _context.SaveChangesAsync();
            }
            return entity;
        }

        public async Task<int> InsertAsync(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            foreach (var entity in entities)
            {
                await InsertAsync(entity, false);
            }

            return saveChange ? await _context.SaveChangesAsync() : 0;
        }

        public TEntity Update(TEntity entity, bool saveChange = false)
        {
            var entityId = entity.Id;
            if (!_dbSet.Any(w => !w.IsDeleted && w.Id == entityId)) { return entity; }
            entity.ModifiedDate = DateTime.UtcNow;
            SetUpdatedByForEntity(entity);
            _context.Update(entity);
            if (saveChange)
            {
                _context.SaveChanges();
            }
            return entity;
        }

        public int Update(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            foreach (var entity in entities)
            {
                Update(entity, false);
            }

            return saveChange ? _context.SaveChanges() : 0;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, bool saveChange = false)
        {
            var entityId = entity.Id;
            if (!_dbSet.Any(w => !w.IsDeleted && w.Id == entityId)) { return entity; }
            entity.ModifiedDate = DateTime.UtcNow;
            SetUpdatedByForEntity(entity);
            _context.Update(entity);
            if (saveChange)
            {
                await _context.SaveChangesAsync();
            }
            return entity;
        }

        public async Task<int> UpdateAsync(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            foreach (var entity in entities)
            {
                await UpdateAsync(entity, false);
            }

            return saveChange ? await _context.SaveChangesAsync() : 0;
        }

        public int Delete(Guid id, bool saveChange = false)
        {
            _ = _dbSet
                .Where(w => w.Id == id && !w.IsDeleted)
                .ExecuteDelete();

            return saveChange ? _context.SaveChanges() : 0;
        }

        public int Delete(TEntity entity, bool saveChange = false)
        {
            return Delete(entity.Id, saveChange);
        }

        public int Delete(Expression<Func<TEntity, bool>> predicate, bool saveChange = false)
        {
            var entities = Query().Where(predicate).ToList();
            return Delete(entities, saveChange);
        }

        public int Delete(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            _dbSet.RemoveRange(entities);
            return saveChange ? _context.SaveChanges() : 0;
        }

        public async Task<int> DeleteAsync(Guid id, bool saveChange = false)
        {
            _ = await _dbSet
                .Where(w => w.Id == id && !w.IsDeleted)
                .ExecuteDeleteAsync();

            return saveChange ? await _context.SaveChangesAsync() : 0;
        }

        public async Task<int> DeleteAsync(TEntity entity, bool saveChange = false)
        {
            return await DeleteAsync(entity.Id, saveChange);
        }

        public async Task<int> DeleteAsync(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                return saveChange ? await _context.SaveChangesAsync() : 0;
            }
            return 0;
        }

        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool saveChange = false)
        {
            var entities = Query().Where(predicate).ToList();
            return await DeleteAsync(entities, saveChange);
        }

        public int SoftDelete(TEntity entity, bool saveChange = false)
        {
            _ = _dbSet
                .Where(w => w.Id == entity.Id && !w.IsDeleted)
                .ExecuteUpdate(s => s.SetProperty(p => p.IsDeleted, true));

            return saveChange ? _context.SaveChanges() : 0;
        }

        public int SoftDelete(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            foreach (var entity in entities)
            {
                SoftDelete(entity, false);
            }

            return saveChange ? _context.SaveChanges() : 0;
        }

        public int SoftDelete(Expression<Func<TEntity, bool>> predicate, bool saveChange = false)
        {
            var entities = Query().Where(predicate).ToList();
            return SoftDelete(entities, saveChange);
        }

        public async Task<int> SoftDeleteAsync(TEntity entity, bool saveChange = false)
        {
            _ = await _dbSet
                 .Where(w => w.Id == entity.Id && !w.IsDeleted)
                 .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDeleted, true));

            return saveChange ? await _context.SaveChangesAsync() : 0;
        }

        public async Task<int> SoftDeleteAsync(IEnumerable<TEntity> entities, bool saveChange = false)
        {
            foreach (var entity in entities)
            {
                await SoftDeleteAsync(entity, false);
            }

            return saveChange ? await _context.SaveChangesAsync() : 0;
        }

        public async Task<int> SoftDeleteAsync(Expression<Func<TEntity, bool>> predicate, bool saveChange = false)
        {
            var entities = Query().Where(predicate).ToList();
            return await SoftDeleteAsync(entities, saveChange);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!)
        {
            IQueryable<TEntity> query = Query(false);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            return query.Any();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null!,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!)
        {
            IQueryable<TEntity> query = Query(false);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            return await query.AnyAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = Query().Where(predicate);
            return query.Count();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = Query().Where(predicate);
            return await query.CountAsync();
        }

        #region Private

        private static void SetCreatedByForEntity(TEntity entity)
        {
            if (entity.CreatedById != null)
            {
                var currentUserId = HttpContextUtils.Identity?.UserId();
                entity.CreatedById = currentUserId;
            }
        }

        private static void SetUpdatedByForEntity(TEntity entity)
        {
            if (entity.ModifiedById != null)
            {
                var currentUserId = HttpContextUtils.Identity?.UserId();
                entity.ModifiedById = currentUserId;
            }
        }

        #endregion
    }
}
