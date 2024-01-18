using PhiThanh.Core;

namespace PhiThanh.DataAccess.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public string? BannerUrl { get; set; }

        public virtual HashSet<Category> Categories { get; set; } = [];
        public virtual HashSet<Tag> Tags { get; set; }= [];
    }
}
