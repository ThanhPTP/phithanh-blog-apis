using PhiThanh.Core;

namespace PhiThanh.DataAccess.Entities
{
    public class PostTag : BaseEntity
    {
        public virtual Guid PostId { get; set; }
        public virtual Post Post { get; set; }

        public virtual Guid TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
