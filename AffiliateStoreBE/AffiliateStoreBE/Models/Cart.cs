using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class Cart : BaseEntity
    {
        public Guid Id { get; set; }
        public String AccountId { get; set; }
        public Account Account { get; set; }
    }
}
