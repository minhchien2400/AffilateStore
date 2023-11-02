using AffiliateStoreBE.DbConnect;

namespace AffiliateStoreBE.Service
{
    public class ProductsService
    {
        private readonly StoreDbContext _storeDbContext;
        public ProductsService(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }
    }
}
