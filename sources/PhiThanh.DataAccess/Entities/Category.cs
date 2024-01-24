using PhiThanh.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhiThanh.DataAccess.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? BannerUrl { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }

        [ForeignKey(nameof(ParentCategory))]
        public virtual Guid? ParentCategoryId { get; set; }
        public virtual Category? ParentCategory { get; set; }
    }
}
