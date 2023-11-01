using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace AffiliateStoreBE.Controllers
{
    public class CategoryController : ControllerBase
    {
        private readonly StoreDbContext _storeDbContext;
        public CategoryController(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }
        [HttpGet("getcategory")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetCategory()
        {
            var category = new List<CategoryModel>();
            try
            {
                category = await _storeDbContext.Set<Category>().Where(a => !a.IsDeleted).Select(a => new CategoryModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Image = a.Image,
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok(new { category, category.Count });
        }

        [HttpPost("createorupdatecategory")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> CreateOrUpdateCategory([FromBody] CategoryModel category)
        {
            try
            {
                var nowTime = DateTime.UtcNow;
                if(category.Id != Guid.Empty)
                {
                    var oldCategory = await _storeDbContext.Set<Category>().Where(c => c.Id == category.Id).FirstOrDefaultAsync();
                    if(oldCategory != null)
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

        [HttpDelete("deletecategory")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> DeleteCategory([FromBody] Guid categoryId)
        {
            try
            {
                var deleteCategory = await _storeDbContext.Set<Category>().Where(a => a.Id == categoryId && !a.IsDeleted).FirstOrDefaultAsync();
                if(deleteCategory != null)
                {
                    deleteCategory.IsDeleted = true;
                    await _storeDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok(true);
        }
        public class CategoryModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
        }
    }
}
