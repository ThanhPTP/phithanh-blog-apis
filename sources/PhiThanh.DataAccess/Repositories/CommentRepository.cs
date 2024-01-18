using PhiThanh.DataAccess.Contexts;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Kernel;

namespace PhiThanh.DataAccess.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
    }

    public class CommentRepository(CoreDataContext context) : GenericRepository<Comment>(context), ICommentRepository
    {
    }
}
