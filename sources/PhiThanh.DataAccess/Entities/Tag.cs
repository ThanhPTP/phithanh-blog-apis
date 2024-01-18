using Microsoft.EntityFrameworkCore;
using PhiThanh.Core;

namespace PhiThanh.DataAccess.Entities
{
    [Index(nameof(Key), IsUnique = true)]
    public class Tag : BaseEntity
    {
        public string Key { get; set; }
    }
}