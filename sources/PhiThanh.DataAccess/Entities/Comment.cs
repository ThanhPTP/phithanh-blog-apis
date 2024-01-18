using PhiThanh.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhiThanh.DataAccess.Entities
{
    public class Comment : BaseEntity
    {
        public string AliasName { get; set; }
        public string? Email { get; set; }
        public string Content { get; set; }
        public int? Rating { get; set; }
        public int Level { get; set; }

        [ForeignKey(nameof(Post))]
        public virtual Guid PostId { get; set; }
        public virtual Post Post { get; set; }

        [ForeignKey(nameof(ParentComment))]
        public virtual Guid? ParentCommentId { get; set; }
        public virtual Comment? ParentComment { get; set; }
    }
}
