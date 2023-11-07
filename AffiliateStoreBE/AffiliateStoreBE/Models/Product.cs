using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class Product : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Cost { get; set; }
        public float Price { get; set; }
        public string Images { get; set; }
        public Guid CategoryId { get; set; }
        public int Stars { get; set; }
        public string AffLink { get; set; }
        public int TotalSales { get; set; }
        public virtual Category Category { get; set; }
        public Status Status { get; set; } = 0;
    }
}
