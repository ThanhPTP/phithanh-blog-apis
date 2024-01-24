using PhiThanh.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhiThanh.DataAccess.Entities
{
    public class PostCategory : BaseEntity
    {
        [ForeignKey(nameof(Post))]
        public virtual Guid? PostId { get; set; }
        public virtual Post? Post { get; set; }

        [ForeignKey(nameof(Category))]
        public virtual Guid? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
}
