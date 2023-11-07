using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using AffiliateStoreBE.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace AffiliateStoreBE.Service
{
    public class CategorysService : ICategoryService
    {
        private readonly StoreDbContext _storeDbContext;
        public CategorysService(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }
        public async Task<List<Category>> GetCategoryByNameAndImage(List<string> categoryNames, List<string> images = null)
        {
            var categories = new List<Category>();
            if (images != null)
            {
                categories = await _storeDbContext.Set<Category>().Where(a => a.Status == Status.Active && ((categoryNames.Contains(a.Name.ToLower()) && !images.Contains(a.Image)) || (!categoryNames.Contains(a.Name.ToLower()) && images.Contains(a.Image)))).ToListAsync();
            }
            else
            {
                categories = await _storeDbContext.Set<Category>().Where(a => a.Status == Status.Active && (categoryNames.Contains(a.Name.ToLower()))).ToListAsync();
            }
            return categories;
        }
    }
}
