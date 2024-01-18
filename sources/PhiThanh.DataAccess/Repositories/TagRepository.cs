using PhiThanh.DataAccess.Contexts;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Kernel;

namespace PhiThanh.DataAccess.Repositories
{
    public interface ITagRepository : IGenericRepository<Tag>
    {
    }

    public class TagRepository(CoreDataContext context) : GenericRepository<Tag>(context), ITagRepository
    {
    }
}
