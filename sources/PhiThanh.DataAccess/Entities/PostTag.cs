using PhiThanh.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhiThanh.DataAccess.Entities
{
    public class PostTag : BaseEntity
    {
        [ForeignKey(nameof(Post))]
        public virtual Guid? PostId { get; set; }
        public virtual Post? Post { get; set; }

        [ForeignKey(nameof(Tag))]
        public virtual Guid? TagId { get; set; }
        public virtual Tag? Tag { get; set; }
    }
}
