using AffiliateStoreBE.Models;

namespace AffiliateStoreBE.Service.IService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoryByNameAndImage(List<string> categoryNames, List<string> images = null);
    }
}
