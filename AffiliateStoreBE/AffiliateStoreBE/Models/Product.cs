namespace AffiliateStoreBE.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Images { get; set; }
        public ProductType Type { get; set; }
    }
    public enum ProductType
    {
        None =  0,
        Fashion = 1,
        Pets = 2,
        Sport = 3,
        Electronics = 4

    }
}
