using static AffiliateStoreBE.Service.ProductsService;

namespace AffiliateStoreBE.Service.IService
{
    public interface IProductsService
    {
        Task<List<ValidateProductName>> CheckProductName(List<string> names);
        Task<List<ValidateProductName>> GetProductsByIds(List<Guid> ids);
    }
}
