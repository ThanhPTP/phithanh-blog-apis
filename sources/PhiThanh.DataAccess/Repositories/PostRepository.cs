using PhiThanh.DataAccess.Contexts;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Kernel;

namespace PhiThanh.DataAccess.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
    }

    public class PostRepository(CoreDataContext context) : GenericRepository<Post>(context), IPostRepository
    {
    }
}
