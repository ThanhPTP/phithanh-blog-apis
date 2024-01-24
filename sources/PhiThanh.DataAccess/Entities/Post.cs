using PhiThanh.Core;
using static PhiThanh.Core.Constants;

namespace PhiThanh.DataAccess.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public string? BannerUrl { get; set; }
        public string Slug { get; set; }
        public DateTime? DisplayDate { get; set; }
        public PostStatus PostStatus { get; set; }
        public virtual HashSet<PostCategory>? Categories { get; set; } = [];
        public virtual HashSet<PostTag>? Tags { get; set; } = [];
    }
}
