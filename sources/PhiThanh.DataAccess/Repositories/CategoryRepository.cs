using PhiThanh.DataAccess.Contexts;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Kernel;

namespace PhiThanh.DataAccess.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
    }

    public class CategoryRepository(CoreDataContext context) : GenericRepository<Category>(context), ICategoryRepository
    {
    }
}
