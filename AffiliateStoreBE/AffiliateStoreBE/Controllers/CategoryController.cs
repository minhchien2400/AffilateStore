using AffiliateStoreBE.Common;
using AffiliateStoreBE.Common.I18N;
using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mail;

namespace AffiliateStoreBE.Controllers
{
    public class CategoryController : ApiBaseController
    {
        private readonly StoreDbContext _storeDbContext;
        public CategoryController(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }
        [HttpPost("getcategory")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetCategory([FromBody] FilterModel filter)
        {
            var categories = new List<CategoryModel>();
            int totalCount = 1;
            try
            {
                categories = await _storeDbContext.Set<Category>().Where(a => a.Status == Status.Active).Select(a => new CategoryModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Image = a.Image,
                }).ToListAsync();
                if (filter.SearchText != String.Empty && filter.SearchText != null)
                {
                    var listCategoriesName = SearchString(filter.SearchText, categories.Select(p => p.Name).ToList());
                    categories = categories.Where(a => listCategoriesName.Contains(a.Name)).OrderBy(a => listCategoriesName.IndexOf(a.Name)).ToList();
                }
                if (filter.Keys != null && filter.Keys.Any(a => a != null))
                {
                    if (filter.Keys.Contains("a-z"))
                    {
                        categories = categories.OrderBy(c => c.Name).ToList();
                    }
                    else if (filter.Keys.Contains("z-a"))
                    {
                        categories = categories.OrderByDescending(c => c.Name).ToList();
                    }
                }
                if (categories.Any() && filter.Limit != 100)
                {
                    totalCount = (int)Math.Ceiling(categories.Count() / (decimal)filter.Limit);
                    categories = DoTake(categories.AsQueryable(), filter).ToList();
                }
                return Ok(new
                {
                    HasError = false,
                    Result = categories,
                    Filter = filter,
                    TotalCount = totalCount == 0 ? 1 : totalCount,
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getcategoryinactive")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetCategoryInactive()
        {
            try
            {
                var category = await _storeDbContext.Set<Category>().Where(a => a.Status == Status.Inactive).Select(a => new CategoryModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Image = a.Image,
                }).ToListAsync();
                return Ok(new { category, category.Count });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("createorupdatecategory")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> CreateOrUpdateCategory([FromBody] CategoryModel category)
        {
            try
            {
                var nowTime = DateTime.UtcNow;
                if (category.Id != Guid.Empty)
                {
                    var oldCategory = await _storeDbContext.Set<Category>().Where(c => c.Status == Status.Active && c.Id == category.Id).FirstOrDefaultAsync();
                    if (oldCategory != null)
                    {
                        oldCategory.Name = category.Name != null ? category.Name : oldCategory.Name;
                        oldCategory.Image = category.Image != null ? category.Image : oldCategory.Image;
                        oldCategory.ModifiedTime = nowTime;
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    var newCategory = new Category()
                    {
                        Id = Guid.NewGuid(),
                        Name = category.Name,
                        Image = category.Image,
                        CreatedTime = nowTime,
                        ModifiedTime = new DateTimeOffset()
                    };
                    await _storeDbContext.AddRangeAsync(newCategory);
                }
                await _storeDbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok(true);
        }

        [HttpPost("deleteorinactivecategory")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> DeleteCategory([FromBody] DeleteOrInactiveCategory deleteOrInactive)
        {
            try
            {
                var oldCategory = await _storeDbContext.Set<Category>().Where(a => a.Id == deleteOrInactive.CategoryId && a.Status == Status.Active).FirstOrDefaultAsync();
                if (oldCategory != null && deleteOrInactive.IsInactive)
                {
                    oldCategory.Status = Status.Inactive;
                }
                else if (oldCategory != null && !deleteOrInactive.IsInactive)
                {
                    oldCategory.Status = Status.Deleted;
                }
                await _storeDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok(true);
        }

        [HttpPost("activecategory")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ActiveCategory(Guid categoryId)
        {
            try
            {
                var category = await _storeDbContext.Set<Category>().Where(a => a.Status == Status.Inactive).FirstOrDefaultAsync();
                if (category != null)
                {
                    category.Status = Status.Active;
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public class CategoryModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
        }
        public class DeleteOrInactiveCategory
        {
            public Guid CategoryId { get; set; }
            public bool IsInactive { get; set; }
        }
    }
}
