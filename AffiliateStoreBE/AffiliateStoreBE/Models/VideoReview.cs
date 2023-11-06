using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class VideoReview
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoLink { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        public Status Status { get; set; }
    }
}
