namespace AffiliateStoreBE.Common.Models
{
    public abstract class BaseEntity
    {
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset ModifiedTime { get; set; }

    }
}
