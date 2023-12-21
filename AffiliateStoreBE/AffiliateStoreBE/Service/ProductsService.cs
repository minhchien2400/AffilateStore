using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Controllers;
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
        public List<ProductModel> GetProductsByFilterKeys(List<ProductModel> products, List<string> keys)
        {

            if (keys.Contains("over-3-stars"))
            {
                products = products.Where(a => (float)(a.Stars / 10) > 3).ToList();
            }
            else if (keys.Contains("over-4-stars"))
            {
                products = products.Where(a => (float)(a.Stars / 10) > 4).ToList();
            }
            if (keys.Contains("price-up"))
            {
                products = products.OrderBy(a => a.Price).ToList();
            }
            else if (keys.Contains("price-down"))
            {
                products = products.OrderByDescending(a => a.Price).ToList();
            }
            else if (keys.Contains("top-sale"))
            {
                products = products.OrderByDescending(a => (int)((a.Price/a.Cost)*100)).ThenByDescending(a => a.Price).ToList();
            }
            return products;
        }
        public List<CartActionResponeModel> GetCartProductsByFilterKeys(List<CartActionResponeModel> products, List<string> keys)
        {
            if (keys.Contains("create-time"))
            {
                products = products.OrderBy(p => p.CreatedTime).ToList();
            }
            else if (keys.Contains("descending-create-time"))
            {
                products = products.OrderByDescending(p => p.CreatedTime).ToList();
            }
            else if (keys.Contains("top-sale"))
            {
                products = products.OrderByDescending(a => (int)((a.Price / a.Cost) * 100)).ThenByDescending(a => a.Price).ToList();
            }
            return products;
        }
        public class ValidateProductName
        {
            public Guid ProductId { get; set; }
            public string Name { get; set; }
        }
    }
}
