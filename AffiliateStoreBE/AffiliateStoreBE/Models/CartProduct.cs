using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class CartProduct : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public String AccountId { get; set; }
        public Account Account { get; set; }
        public CartStatus Status { get; set; }
    }

    public enum CartStatus
    {
        Added = 0,
        Purchased = 1,
        Removed = 2,
    }
}
