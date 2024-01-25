using PhiThanh.Core;

namespace PhiThanh.DataAccess.Entities
{
    public class PostCategory : BaseEntity
    {
        public virtual Guid PostId { get; set; }
        public virtual Post Post { get; set; }

        public virtual Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
