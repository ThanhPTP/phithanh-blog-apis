using PhiThanh.DataAccess.Contexts;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Kernel;

namespace PhiThanh.DataAccess.Repositories
{
    public interface IPostTagRepository : IGenericRepository<PostTag>
    {
    }

    public class PostTagRepository(CoreDataContext context) : GenericRepository<PostTag>(context), IPostTagRepository
    {
    }
}
