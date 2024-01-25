using PhiThanh.DataAccess.Contexts;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Kernel;

namespace PhiThanh.DataAccess.Repositories
{
    public interface IPostCategoryRepository : IGenericRepository<PostCategory>
    {
    }

    public class PostCategoryRepository(CoreDataContext context) : GenericRepository<PostCategory>(context), IPostCategoryRepository
    {
    }
}
