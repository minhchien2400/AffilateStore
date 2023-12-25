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
using System.Linq;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using AvePoint.Confucius.FeatureCommon.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Aspose.Cells.Revisions;

namespace AffiliateStoreBE.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly StoreDbContext _storeContext;
        private readonly ICategoryService _categoryService;
        private readonly IProductsService _productService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<Account> _userManager;
        private readonly IConfiguration _configuration;
        public ProductController(StoreDbContext storeContext, ICategoryService categoryService, IHttpContextAccessor contextAccessor, UserManager<Account> userManager, IConfiguration configuration)
        {
            _storeContext = storeContext;
            _categoryService = categoryService;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("getproducts")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductsByType([FromBody] FilterModel filter)
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
                    Images = !string.IsNullOrEmpty(a.Images) ? a.Images.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>(),
                    CategoryName = a.Category.Name,
                    Stars = a.Stars,
                    AffLink = a.AffLink,
                    TotalSales = a.TotalSales,
                    CategoryId = a.Category.Id,
                }).ToListAsync();
                if (filter.SearchText != String.Empty && filter.SearchText != null)
                {
                    var listProductsName = SearchString(filter.SearchText, products.Select(p => p.ProductName).ToList());
                    products = products.Where(a => listProductsName.Contains(a.ProductName)).OrderBy(a => listProductsName.IndexOf(a.ProductName)).ToList();
                }
                if (filter.Keys != null && filter.Keys.Any(a => a != null))
                {
                    if (filter.Keys.Contains("over-3-stars"))
                    {
                        products = products.Where(a => (float)(a.Stars / 10) > 3).ToList();
                    }
                    else if (filter.Keys.Contains("over-4-stars"))
                    {
                        products = products.Where(a => (float)(a.Stars / 10) > 4).ToList();
                    }
                    if (filter.Keys.Contains("price-up"))
                    {
                        products = products.OrderBy(a => a.Price).ToList();
                    }
                    else if (filter.Keys.Contains("price-down"))
                    {
                        products = products.OrderByDescending(a => a.Price).ToList();
                    }
                    else if (filter.Keys.Contains("top-sale"))
                    {
                        products = products.OrderBy(a => (int)((a.Price / a.Cost) * 100)).ThenByDescending(a => a.Price).ToList();
                    }
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
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet("getproductbyid/{id}")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var product = await _storeContext.Set<Product>().Where(a => a.Id == id && a.Status == Status.Active).Select(a => new ProductModel
                {
                    ProductId = a.Id,
                    ProductName = a.Name,
                    Description = a.Description,
                    Cost = a.Cost,
                    Price = a.Price,
                    Images = !string.IsNullOrEmpty(a.Images) ? a.Images.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>(),
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
        public async Task<IActionResult> GetProductsByCategoryName(ProductsCategoryNameFilter filter)
        {
            try
            {
                var products = new List<ProductModel>();
                int totalCount = 1;
                if (filter.CategoryName != String.Empty && filter.CategoryName != null)
                {
                    products = await _storeContext.Set<Product>().Where(a => filter.CategoryName.Equals(a.Category.Name) && a.Status == Status.Active).Select(a => new ProductModel
                    {
                        ProductId = a.Id,
                        ProductName = a.Name,
                        Description = a.Description,
                        Cost = a.Cost,
                        Price = a.Price,
                        Images = !string.IsNullOrEmpty(a.Images) ? a.Images.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>(),
                        CategoryName = a.Category.Name,
                        Stars = a.Stars,
                        AffLink = a.AffLink,
                        TotalSales = a.TotalSales
                    }).ToListAsync();
                }

                if (filter.Keys != null)
                {
                    products = _productService.GetProductsByFilterKeys(products, filter.Keys);
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
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("category/{categoryId}")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductsByCategoryId([FromBody] ProductsCategoryIdFilter filter)
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
                        Images = !string.IsNullOrEmpty(a.Images) ? a.Images.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>(),
                        CategoryName = a.Category.Name,
                        Stars = a.Stars,
                        AffLink = a.AffLink,
                        TotalSales = a.TotalSales
                    }).ToListAsync();
                }
                if (filter.SearchText != String.Empty && filter.SearchText != null)
                {
                    var listProductsName = SearchString(filter.SearchText, products.Select(p => p.ProductName).ToList());
                    products = products.Where(a => listProductsName.Contains(a.ProductName)).OrderBy(a => listProductsName.IndexOf(a.ProductName)).ToList();
                }
                if (filter.Keys != null)
                {
                    if (filter.Keys.Contains("over-3-stars"))
                    {
                        products = products.Where(a => (a.Stars / 10) - 3 >= 0).ToList();
                    }
                    else if (filter.Keys.Contains("over-4-stars"))
                    {
                        products = products.Where(a => (a.Stars / 10) - 4 >= 0).ToList();
                    }
                    if (filter.Keys.Contains("price-up"))
                    {
                        products = products.OrderBy(a => a.Price).ToList();
                    }
                    else if (filter.Keys.Contains("price-down"))
                    {
                        products = products.OrderByDescending(a => a.Price).ToList();
                    }
                    else if (filter.Keys.Contains("top-sale"))
                    {
                        products = products.OrderByDescending(a => (int)((a.Price / a.Cost) * 100)).ThenByDescending(a => a.Price).ToList();
                    }
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
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getproductinactive")]
        [SwaggerResponse(200)]
        [Authorize("RequireAdminRole")]
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
                    Images = !string.IsNullOrEmpty(a.Images) ? a.Images.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>(),
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
                        product.Images = pr.Images != null ? String.Join("; ", pr.Images) : String.Empty;
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
                    product.Images = String.Join("; ", pr.Images);
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

        [HttpPost("cartaction")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> CartAction([FromBody] CartActionModel addToCart)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(addToCart.AccessToken);
                if (principal?.Identity?.Name is null)
                    return Unauthorized();

                var user = await _userManager.FindByNameAsync(principal.Identity.Name);
                var product = await _storeContext.Set<Product>().Where(a => a.Id == addToCart.ProductId && a.Status == Status.Active).Select(a => new CartActionResponeModel
                {
                    ProductId = a.Id,
                    ProductName = a.Name,
                    Cost = a.Cost,
                    Price = a.Price,
                    Images = a.Images,
                    AffLink = a.AffLink,
                }).FirstOrDefaultAsync();
                if (product != null)
                {
                    var cartProducts = await _storeContext.Set<CartProduct>().Where(a => a.ProductId == addToCart.ProductId).ToListAsync();
                    if (addToCart.ActionType == ActionType.Add)
                    {
                        if (cartProducts.Where(c => c.Status == CartStatus.Added).Any())
                        {
                            return Ok(new ApiRespone<CartActionResponeModel>
                            {
                                IsSuccess = false,
                                Message = "SP da co trong gio hang",
                            });
                        }
                        await _storeContext.AddAsync(new CartProduct
                        {
                            Id = Guid.NewGuid(),
                            ProductId = addToCart.ProductId,
                            AccountId = user.Id,
                            Status = CartStatus.Added
                        });
                        await _storeContext.SaveChangesAsync();
                        return Ok(new ApiRespone<CartActionResponeModel>
                        {
                            IsSuccess = true,
                            Message = "1 SP da duoc them vao gio hang",
                            Result = product
                        });
                    }
                    else
                    {
                        //if (cartProducts.Where(c => c.Status != CartStatus.Removed).Any())
                        //{
                        var hasCartAdded = cartProducts.Where(c => c.Status == CartStatus.Added).Any();
                        var hasCartPurchased = cartProducts.Where(c => c.Status == CartStatus.Purchased).Any();
                        if (addToCart.ActionType == ActionType.Remove)
                        {
                            if (addToCart.IsCart && hasCartAdded)
                            {
                                cartProducts.Where(c => c.Status == CartStatus.Added).FirstOrDefault().Status = CartStatus.Removed;
                                await _storeContext.SaveChangesAsync();
                                return Ok(new ApiRespone<CartActionResponeModel>
                                {
                                    IsSuccess = true,
                                    Message = "Da go SP khoi gio hang",
                                    Result = product
                                });
                            }
                            if (!addToCart.IsCart && hasCartPurchased)
                            {
                                cartProducts.Where(c => c.Status == CartStatus.Purchased).FirstOrDefault().Status = CartStatus.Removed;
                                await _storeContext.SaveChangesAsync();
                                return Ok(new ApiRespone<CartActionResponeModel>
                                {
                                    IsSuccess = true,
                                    Message = "Da go SP khoi danh sach san pham da mua",
                                    Result = product
                                });
                            }
                        }
                        else if(hasCartAdded)
                        {
                            cartProducts.Where(c => c.Status == CartStatus.Added).FirstOrDefault().Status = CartStatus.Purchased;
                            await _storeContext.SaveChangesAsync();
                            return Ok(new ApiRespone<CartActionResponeModel>
                            {
                                IsSuccess = true,
                                Message = "Danh dau da mua SP",
                                Result = product
                            });
                        }
                        
                        return Ok(new ApiRespone<CartActionResponeModel>
                        {
                            IsSuccess = false,
                            Message = "SP khong co trong gio hang",
                        });
                    }
                }
                return Ok(new ApiRespone<CartActionResponeModel>
                {
                    IsSuccess = false,
                    Message = "SP khong ton tai hoac da bi xoa",
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("getcartproducts")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetProductsPurchased([FromBody] CartProductFilterModel filter)
        {
            try
            {
                int totalCount = 1;
                var principal = GetPrincipalFromExpiredToken(filter.AccessToken);
                if (principal?.Identity?.Name is null)
                    return Unauthorized();

                var user = await _userManager.FindByNameAsync(principal.Identity.Name);
                var cartProduct = await _storeContext.Set<CartProduct>().Include(a => a.Product).Where(a => a.AccountId.Equals(user.Id) && a.Status == filter.CartStatus).Select(c => new CartActionResponeModel
                {
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    Cost = c.Product.Cost,
                    Price = c.Product.Price,
                    Images = c.Product.Images,
                    AffLink = c.Product.AffLink,
                    CreatedTime = c.CreatedTime,
                }).ToListAsync();

                if (cartProduct.Any())
                {
                    if (filter.SearchText != String.Empty && filter.SearchText != null)
                    {
                        var listProductsName = SearchString(filter.SearchText, cartProduct.Select(p => p.ProductName).ToList());
                        cartProduct = cartProduct.Where(a => listProductsName.Contains(a.ProductName)).OrderBy(a => listProductsName.IndexOf(a.ProductName)).ToList();
                    }
                    if (filter.Keys != null && filter.Keys.Any(a => a != null) && !filter.Keys.Contains("all"))
                    {
                        if (filter.Keys.Contains("time-up"))
                        {
                            cartProduct = cartProduct.OrderBy(a => a.CreatedTime).ToList();
                        }
                        else if (filter.Keys.Contains("time-down"))
                        {
                            cartProduct = cartProduct.OrderByDescending(a => a.CreatedTime).ToList();
                        }
                        else if (filter.Keys.Contains("top-sale"))
                        {
                            cartProduct = cartProduct.OrderBy(a => (int)((a.Price / a.Cost) * 100)).ThenByDescending(a => a.Price).ToList();
                        }
                    }
                    totalCount = (int)Math.Ceiling(cartProduct.Count() / (decimal)filter.Limit);
                    cartProduct = DoTake(cartProduct.AsQueryable(), filter).ToList();
                }
                return Ok(new
                {
                    Products = cartProduct,
                    TotalProducts = cartProduct != null ? cartProduct.Count() : 0,
                    Filter = filter,
                    TotalCount = totalCount == 0 ? 1 : totalCount,
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("gettotalcart")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetTotalCarts([FromBody] string accessToken)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(accessToken);
                if (principal?.Identity?.Name is null)
                    return Unauthorized();

                var user = await _userManager.FindByNameAsync(principal.Identity.Name);
                var totalAded = await _storeContext.Set<CartProduct>().Where(a => a.AccountId.Equals(user.Id) && a.Status == CartStatus.Added).CountAsync();
                return Ok(totalAded);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var secret = _configuration["JWT:Secret"] ?? throw new InvalidOperationException("Secret not configured");

                var validation = new TokenValidationParameters
                {
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateLifetime = false
                };

                var principal = new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
                return principal;
            }
            catch (Exception ex)
            {
                throw ex;
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
        public List<string> Images { get; set; }
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

    public class ProductsCategoryIdFilter : FilterModel
    {
        public Guid CategoryId { get; set; }
    }

    public class ProductsCategoryNameFilter : FilterModel
    {
        public string CategoryName { get; set; }
    }

    public class RemoveOrPurchaseModel
    {
        public Guid productId { get; set; }
        public CartStatus CartStatus { get; set; }
    }

    public class CartActionModel
    {
        public Guid ProductId { get; set; }
        public string AccessToken { get; set; }
        public ActionType ActionType { get; set; }
        public bool IsCart { get; set; } = true;
    }

    public enum ActionType
    {
        Add = 0,
        Purchase = 1,
        Remove = 2,
    }

    public class CartActionResponeModel : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public float Cost { get; set; }
        public float Price { get; set; }
        public string Images { get; set; }
        public string AffLink { get; set; }
    }

    public class CartProductFilterModel : FilterModel
    {
        public string AccessToken { get; set; }
        public CartStatus CartStatus { get; set; }
    }
}
