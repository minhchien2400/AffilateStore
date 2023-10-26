using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace AffiliateStoreBE.Controllers
{
    public class ProductController : ControllerBase
    {
        private readonly StoreDbContext _storeContext;
        public ProductController(StoreDbContext profileDbContext, StoreDbContext storeContext)
        {
            _storeContext = storeContext;
        }

        [HttpPost("getproductsbytype")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductsByType([FromBody] ProductType type)
        {
            try
            {
                var products = await _storeContext.Set<Product>().Where(a => a.Type == type).Select(a => new ProductModel
                {
                    ProductName = a.Name,
                    Description = a.Description,
                    Price = a.Price,
                }).ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok("Action Failed");
        }

        [HttpPost("getproductbyid")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductById([FromBody] Guid productId)
        {
            try
            {
                var profile = await _storeContext.Set<Product>().Where(a => a.Id == productId).Select(a => new ProductModel
                {
                    ProductName = a.Name,
                    Description = a.Description,
                    Price = a.Price,
                }).FirstOrDefaultAsync();
                return Ok(profile);
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok("Action Failed");
        }
    }
    public class ProductModel
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
    }
}
