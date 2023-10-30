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
                category = await _storeDbContext.Set<Category>().Select(a => new CategoryModel
                {
                    CategoryName = a.Name,
                    Image = a.Image,
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok(new { category, category.Count });
        }
        public class CategoryModel
        {
            public string CategoryName { get; set; }
            public string Image { get; set; }
        }
    }
}
