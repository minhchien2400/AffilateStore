using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Models
{
    public class Product : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public double Price { get; set; }
        public string Images { get; set; }
        public Guid CategoryId { get; set; }
        public int Stars { get; set; }
        public string AffLink { get; set; }
        public virtual Category Category { get; set; }
        public Status Status { get; set; } = 0;
    }

    public class ImportRequest
    {
        public IFormFile ImportFile { get; set; }

        public String Language { get; set; }
    }

    public class ImportPathInfo
    {
        public string FileName { get; set; }
        public byte[] ImportFileBytes { get; set; }
        public string Language { get; set; }
    }
}
