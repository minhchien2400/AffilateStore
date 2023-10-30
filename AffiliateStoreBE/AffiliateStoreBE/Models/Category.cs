using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class Category : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public Type Type { get; set; }
    }
    public enum Type
    {
        None = 0,
        Fashion = 1,
        Pets = 2,
        Sport = 3,
        Electronics = 4
    }
}
