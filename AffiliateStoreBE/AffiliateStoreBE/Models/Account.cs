using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class Account : BaseEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password  { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string Country { get; set; }
    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2,
    }
}
