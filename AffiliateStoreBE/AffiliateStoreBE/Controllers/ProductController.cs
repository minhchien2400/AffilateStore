using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Status = AffiliateStoreBE.Common.Models.Status;

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
        public async Task<IActionResult> GetProductsByType([FromBody] Guid categoryId)
        {
            try
            {
                var products = await _storeContext.Set<Product>().Include(a => a.Category).Where(a => a.Id == categoryId && a.Status == Status.Active).Select(a => new ProductModel
                {
                    ProductId = a.Id,
                    ProductName = a.Name,
                    Description = a.Description,
                    Cost = a.Cost,
                    Price = a.Price,
                    Images = a.Images,
                    CategoryId = a.CategoryId,
                    Stars = a.Stars,
                    AffLink = a.AffLink,
                }).ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("getproductbyid")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductById([FromBody] Guid productId)
        {
            try
            {
                var profile = await _storeContext.Set<Product>().Where(a => a.Id == productId &&  a.Status == Status.Active).Select(a => new ProductModel
                {
                    ProductId = a.Id,
                    ProductName = a.Name,
                    Description = a.Description,
                    Cost= a.Cost,
                    Price = a.Price,
                    Images = a.Images,
                    CategoryId = a.CategoryId,
                    Stars = a.Stars,
                    AffLink = a.AffLink
                }).FirstOrDefaultAsync();
                return Ok(profile);
            }
            catch (Exception ex)
            {
                throw;
            }
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
                    product = await _storeContext.Set<Product>().Where(a => a.Id == pr.ProductId && a.Status == Status.Active).FirstOrDefaultAsync();
                    if(product != null)
                    {
                        product.Name = pr.ProductName != null ? pr.ProductName : product.Name;
                        product.Description = pr.Description != string.Empty ? pr.Description : product.Description;
                        product.Cost = pr.Cost != 0 ? pr.Cost : product.Cost;
                        product.Price = pr.Price != 0 ? pr.Price : product.Price;
                        product.Images = pr.Images != null ? pr.Images : product.Images;
                        product.CategoryId = pr.CategoryId;
                        product.Stars = pr.Stars != 0 ? pr.Stars : product.Stars;
                        product.AffLink = pr.AffLink != null ? pr.AffLink : product.AffLink;
                        product.ModifiedTime = timeNow;
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    product.Id = Guid.NewGuid();
                    product.Name = pr.ProductName;
                    product.Description = pr.Description;
                    product.Cost = pr.Cost;
                    product.Images = pr.Images;
                    product.CategoryId = pr.CategoryId;
                    product.Stars = pr.Stars;
                    product.AffLink = pr.AffLink;
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

        [HttpDelete("deleteorinactiveproduct")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> DeleteProduct([FromBody] DeleteOrInactiveProduct deleteOrInactive)
        {
            try
            {
                if (deleteOrInactive.ProductId != Guid.Empty)
                {
                    var productInDb = await _storeContext.Set<Product>().Where(a => a.Id == deleteOrInactive.ProductId && a.Status == Status.Active).FirstOrDefaultAsync();
                    if(productInDb != null && deleteOrInactive.IsInactive)
                    {
                        productInDb.Status = Status.Inactive;
                    }
                    else if (productInDb != null && !deleteOrInactive.IsInactive)
                    {
                        productInDb.Status = Status.Deleted;
                    }
                    await _storeContext.SaveChangesAsync();
                }
            }
            catch(Exception)
            {
                throw;
            }
            return Ok(true);
        }

    }
    public class ProductModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public double Price { get; set; }
        public string Images { get; set; }
        public Guid CategoryId { get; set; }
        public int Stars { get; set; }
        public string AffLink { get; set; }
        public Status Status { get; set; }
    }
    public class DeleteOrInactiveProduct
    {
        public Guid ProductId { get; set;}
        public bool IsInactive { get; set; }
    }

}
