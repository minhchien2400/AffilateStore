using static AffiliateStoreBE.Service.ProductsService;

namespace AffiliateStoreBE.Service
{
    public interface IProductsService
    {
        Task<List<ValidateProductName>> CheckProductName(List<string> names);
    }
}
