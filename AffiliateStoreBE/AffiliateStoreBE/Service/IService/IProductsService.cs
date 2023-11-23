using AffiliateStoreBE.Controllers;
using AffiliateStoreBE.Models;
using static AffiliateStoreBE.Service.ProductsService;

namespace AffiliateStoreBE.Service.IService
{
    public interface IProductsService
    {
        Task<List<ValidateProductName>> CheckProductName(List<string> names);
        Task<List<ValidateProductName>> GetProductsByIds(List<Guid> ids);
        List<ProductModel> GetProductsByFilterKeys(List<ProductModel> products, List<string> keys);
    }
}
