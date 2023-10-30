using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public async Task<IActionResult> GetProductsByType([FromBody] int type)
        {
            try
            {
                var products = await _storeContext.Set<Product>().Include(a => a.Category).Where(a => ((int)a.Category.Type) == type).Select(a => new ProductModel
                {
                    ProductId = a.Id,
                    ProductName = a.Name,
                    Description = a.Description,
                    Price = a.Price,
                    Images = a.Images,
                    Category = a.Category
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
                    ProductId = a.Id,
                    ProductName = a.Name,
                    Description = a.Description,
                    Price = a.Price,
                    Images = a.Images,
                    Category = a.Category,
                }).FirstOrDefaultAsync();
                return Ok(profile);
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok("Action Failed");
        }

        [HttpPost("createorupdateproduct")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> CreateOrUpdate([FromBody] ProductModel pr)
        {
            try
            {
                var timeNow = DateTime.UtcNow;
                var product = new Product();
                if (pr.ProductId != Guid.Empty)
                {
                    product = await _storeContext.Set<Product>().Where(a => a.Id == pr.ProductId).FirstOrDefaultAsync();
                    product.Id = pr.ProductId;
                    product.Description = pr.Description != string.Empty ? pr.Description : product.Description;
                    product.Price = pr.Price != 0 ? pr.Price : product.Price;
                    product.Images = pr.Images != null ? pr.Images : product.Images;
                    product.Category = new Category
                    {
                        Id = product.Category.Id,
                        Name = pr.Category.Name != null ? pr.Category.Name : product.Category.Name,
                        Image = pr.Category.Image != null ? pr.Category.Image : product.Category.Image,
                        CreatedTime = product.CreatedTime,
                        ModifiedTime = timeNow,
                    };
                    product.ModifiedTime = timeNow;
                }
                else
                {
                    product.Id = Guid.NewGuid();
                    product.Name = pr.ProductName;
                    product.Description = pr.Description;
                    product.Price = pr.Price;
                    product.Images = pr.Images;
                    product.Category = new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = pr.Category.Name,
                        Image = pr.Category.Image,
                        CreatedTime = timeNow,
                        ModifiedTime = new DateTimeOffset()
                    };
                    product.CreatedTime = timeNow;
                    product.ModifiedTime = new DateTimeOffset();
                    await _storeContext.AddAsync(product);
                }
                await _storeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok(true);
        }
    }
    public class ProductModel : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Images { get; set; }
        public Category Category { get; set; }
    }
}
