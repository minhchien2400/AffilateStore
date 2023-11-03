using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Microsoft.EntityFrameworkCore;

namespace AffiliateStoreBE.Service
{
    public class ProductsService : IProductsService
    {
        private readonly StoreDbContext _storeDbContext;
        public ProductsService(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }
        public async Task<List<ValidateProductName>> CheckProductName(List<string> names)
        {
            var productIds = await _storeDbContext.Set<Product>().Where(a => names.Contains(a.Name)).Select(a => new ValidateProductName { ProductId = a.Id, Name = a.Name }).ToListAsync();
            return productIds;
        }
        public class ValidateProductName
        {
            public Guid ProductId { get; set; }
            public string Name { get; set; }
        }
    }
}
