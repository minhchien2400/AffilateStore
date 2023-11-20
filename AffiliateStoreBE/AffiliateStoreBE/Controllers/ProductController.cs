using AffiliateStoreBE.Common;
using AffiliateStoreBE.Common.I18N;
using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using AffiliateStoreBE.Service.IService;
using LinqToExcel.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Status = AffiliateStoreBE.Common.Models.Status;

namespace AffiliateStoreBE.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly StoreDbContext _storeContext;
        private readonly ICategoryService _categoryService;
        public ProductController(StoreDbContext storeContext, ICategoryService categoryService)
        {
            _storeContext = storeContext;
            _categoryService = categoryService;
        }

        [HttpPost("getallproducts")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductsByType([FromBody] FilterModel filterModel)
        {
            try
            {
                int totalCount = 1;
                var products = await _storeContext.Set<Product>().Where(a => a.Status == Status.Active).Select(a => new ProductModel
                {
                    ProductId = a.Id,
                    ProductName = a.Name,
                    Description = a.Description,
                    Cost = a.Cost,
                    Price = a.Price,
                    Images = a.Images,
                    CategoryName = a.Category.Name,
                    Stars = a.Stars,
                    AffLink = a.AffLink,
                    TotalSales = a.TotalSales,
                }).ToListAsync();
                if(filterModel.SearchText != String.Empty)
                {
                    var listProductsName = SearchString(filterModel.SearchText, products.Select(p => p.ProductName).ToList());
                    products = products.Where(a => listProductsName.Contains(a.ProductName)).ToList();
                }
                if(products.Any())
                {
                    totalCount = (int)Math.Ceiling(products.Count() / (decimal)filterModel.Limit);
                    products = DoTake(products.AsQueryable(), filterModel).ToList();
                }
                return Ok(new
                {
                    HasError = false,
                    Result = products,
                    Filter = filterModel,
                    TotalCount = totalCount == 0 ? 1 : totalCount,
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getproductsbytype")]
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
                    CategoryName = a.Category.Name,
                    Stars = a.Stars,
                    AffLink = a.AffLink,
                    TotalSales = a.TotalSales,
                }).ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getproductbyid")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductById([FromBody] Guid productId)
        {
            try
            {
                var product = await _storeContext.Set<Product>().Where(a => a.Id == productId && a.Status == Status.Active).Select(a => new ProductModel
                {
                    ProductId = a.Id,
                    ProductName = a.Name,
                    Description = a.Description,
                    Cost = a.Cost,
                    Price = a.Price,
                    Images = a.Images,
                    CategoryName = a.Category.Name,
                    Stars = a.Stars,
                    AffLink = a.AffLink,
                    TotalSales = a.TotalSales
                }).FirstOrDefaultAsync();
                return Ok(product);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getproductsbycategoryname")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductsByCategoryName(string categoryName)
        {
            try
            {
                var products = new List<ProductModel>();
                var category = await _categoryService.GetCategoryByName(new List<string> { categoryName });
                if (category != null)
                {
                    products = await _storeContext.Set<Product>().Where(a => category.Select(c => c.Id).FirstOrDefault().Equals(a.CategoryId) && a.Status == Status.Active).Select(a => new ProductModel
                    {
                        ProductId = a.Id,
                        ProductName = a.Name,
                        Description = a.Description,
                        Cost = a.Cost,
                        Price = a.Price,
                        Images = a.Images,
                        CategoryName = a.Category.Name,
                        Stars = a.Stars,
                        AffLink = a.AffLink,
                        TotalSales = a.TotalSales
                    }).ToListAsync();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("category/{id}")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductsByCategoryId(ProductCategoryFilter filter)
        {
            try
            {
                var products = new List<ProductModel>();
                int totalCount = 1;
                if (filter.CategoryId != Guid.Empty)
                {
                    products = await _storeContext.Set<Product>().Where(a => a.CategoryId == filter.CategoryId && a.Status == Status.Active).Select(a => new ProductModel
                    {
                        ProductId = a.Id,
                        ProductName = a.Name,
                        Description = a.Description,
                        Cost = a.Cost,
                        Price = a.Price,
                        Images = a.Images,
                        CategoryName = a.Category.Name,
                        Stars = a.Stars,
                        AffLink = a.AffLink,
                        TotalSales = a.TotalSales
                    }).ToListAsync();
                    if (filter.SearchText != String.Empty)
                    {
                        var listProductsName = SearchString(filter.SearchText, products.Select(p => p.ProductName).ToList());
                        products = products.Where(a => listProductsName.Contains(a.ProductName)).ToList();
                    }
                    if (products.Any())
                    {
                        totalCount = (int)Math.Ceiling(products.Count() / (decimal)filter.Limit);
                        products = DoTake(products.AsQueryable(), filter).ToList();
                    }
                    return Ok(new
                    {
                        HasError = false,
                        Result = products,
                        Filter = filter,
                        TotalCount = totalCount == 0 ? 1 : totalCount,
                    });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getproductinactive")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductsInactive(Guid productId)
        {
            try
            {
                var product = await _storeContext.Set<Product>().Where(a => a.Id == productId && a.Status == Status.Inactive).Select(a => new ProductModel
                {
                    ProductId = a.Id,
                    ProductName = a.Name,
                    Description = a.Description,
                    Cost = a.Cost,
                    Price = a.Price,
                    Images = a.Images,
                    CategoryName = a.Category.Name,
                    Stars = a.Stars,
                    AffLink = a.AffLink,
                    TotalSales = a.TotalSales
                }).FirstOrDefaultAsync();
                return Ok(product);
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
                    if (product != null)
                    {
                        product.Name = pr.ProductName != null ? pr.ProductName : product.Name;
                        product.Description = pr.Description != string.Empty ? pr.Description : product.Description;
                        product.Cost = pr.Cost != 0 ? pr.Cost : product.Cost;
                        product.Price = pr.Price != 0 ? pr.Price : product.Price;
                        product.Images = pr.Images != null ? pr.Images : product.Images;
                        product.CategoryId = pr.CategoryId;
                        product.Stars = pr.Stars != 0 ? pr.Stars : product.Stars;
                        product.AffLink = pr.AffLink != null ? pr.AffLink : product.AffLink;
                        product.TotalSales = pr.TotalSales != 0 ? pr.TotalSales : product.TotalSales;
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
                    product.TotalSales = pr.TotalSales;
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

        [HttpPost("deleteorinactiveproduct")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> DeleteProduct([FromBody] DeleteOrInactiveProduct deleteOrInactive)
        {
            try
            {
                if (deleteOrInactive.ProductId != Guid.Empty)
                {
                    var productInDb = await _storeContext.Set<Product>().Where(a => a.Id == deleteOrInactive.ProductId && a.Status == Status.Active).FirstOrDefaultAsync();
                    if (productInDb != null && deleteOrInactive.IsInactive)
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
            catch (Exception)
            {
                throw;
            }
            return Ok(true);
        }

        [HttpPost("activeproduct")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ActiveProduct(Guid productId)
        {
            try
            {
                var product = await _storeContext.Set<Product>().Where(a => a.Id == productId && a.Status == Status.Inactive).FirstOrDefaultAsync();
                if (product != null)
                {
                    product.Status = Status.Active;
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
    public class ProductModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public float Cost { get; set; }
        public float Price { get; set; }
        public string Images { get; set; }
        public string CategoryName { get; set; }
        public int Stars { get; set; }
        public string AffLink { get; set; }
        public int TotalSales { get; set; }
        public Guid CategoryId { get; set; }
    }
    public class DeleteOrInactiveProduct
    {
        public Guid ProductId { get; set; }
        public bool IsInactive { get; set; }
    }

    public class ProductCategoryFilter : FilterModel
    {
        public Guid CategoryId { get; set; }
    }
}
