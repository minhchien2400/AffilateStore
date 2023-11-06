using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
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
        public async Task<List<Category>> GetCategoryByName(List<string> categoryNames)
        {
            var category = await _storeDbContext.Set<Category>().Where(a => a.Status == Status.Active && categoryNames.Select(c => c.ToLower()).ToList().Contains(a.Name.ToLower())).ToListAsync();
            if (category == null)
            {
                return new List<Category>();
            }
            return category;
        }
    }
}
