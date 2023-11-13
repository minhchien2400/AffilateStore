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
        public async Task<List<Category>> GetCategoryByName(List<string> categoryNames)
        {
            var categories = new List<Category>();

            categories = await _storeDbContext.Set<Category>().Where(a => a.Status == Status.Active && (categoryNames.Contains(a.Name))).ToListAsync();
            return categories;
        }
    }
}
