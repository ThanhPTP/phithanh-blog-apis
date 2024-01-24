using Microsoft.EntityFrameworkCore;
using PhiThanh.DataAccess.Entities;

namespace PhiThanh.DataAccess.Contexts
{
    public class CoreDataContext(DbContextOptions<CoreDataContext> options) : DbContext(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
