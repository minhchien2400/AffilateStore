using AffiliateStoreBE.Models;

namespace AffiliateStoreBE.Service
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoryByName(List<string> categoryNames);
    }
}
