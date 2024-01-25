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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Post>()
                .HasMany(e => e.Categories)
                .WithOne(e => e.Post)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Post>()
                .HasMany(e => e.Tags)
                .WithOne(e => e.Post)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
