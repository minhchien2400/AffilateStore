using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class Product : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Images { get; set; }
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
