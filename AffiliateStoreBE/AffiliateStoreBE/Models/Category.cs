using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class Category : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
