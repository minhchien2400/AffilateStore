using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using AffiliateStoreBE.Service.IService;
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
            var products = await _storeDbContext.Set<Product>().Where(a => names.Contains(a.Name)).Select(a => new ValidateProductName { ProductId = a.Id, Name = a.Name }).ToListAsync();
            return products;
        }
        public async Task<List<ValidateProductName>> GetProductsByIds(List<Guid> ids)
        {
            var products = await _storeDbContext.Set<Product>().Where(a => ids.Contains(a.Id)).Select(a => new ValidateProductName { ProductId = a.Id, Name = a.Name }).ToListAsync();
            return products;
        }
        public class ValidateProductName
        {
            public Guid ProductId { get; set; }
            public string Name { get; set; }
        }
    }
}
