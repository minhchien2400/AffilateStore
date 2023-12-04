using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class CartProduct : BaseEntity
    {
        public Guid CartProductId { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
