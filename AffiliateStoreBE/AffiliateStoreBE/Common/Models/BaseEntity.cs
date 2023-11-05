namespace AffiliateStoreBE.Common.Models
{
    public abstract class BaseEntity
    {
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset ModifiedTime { get; set; }
    }

    public enum Status
    {
        Active = 0,
        Inactive = 1,
        Deleted = 2,
    }
}
