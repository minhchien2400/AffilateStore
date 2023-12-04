using AffiliateStoreBE.DbConnect;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.RegularExpressions;
using System.Text;
using AffiliateStoreBE.Common;
using AffiliateStoreBE.Models;
using AffiliateStoreBE.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace AffiliateStoreBE.Controllers
{
    public class HomeController : ApiBaseController
    {
        private readonly StoreDbContext _storeDbContext;
        public HomeController(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }

        [HttpGet("topsale")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetTopSaleProducts(FilterModel filter)
        {
            try
            {
                int totalCount = 1;
                var products = await _storeDbContext.Set<Product>().Where(a => a.Status == Status.Active).OrderByDescending(a => (int)((a.Price / a.Cost) * 100)).ThenByDescending(a => a.Price).Take(50).ToListAsync();
                if (products.Any())
                {
                    totalCount = (int)Math.Ceiling(products.Count() / (decimal)filter.Limit);
                    products = DoTake(products.AsQueryable(), filter).ToList();
                }
                return Ok(new
                {
                    HasError = false,
                    Result = products,
                    Filter = filter,
                    TotalCount = totalCount == 0 ? 1 : totalCount,
                });
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public class ProductSearch
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        public class SearchFilter
        {
            public string SearchText { get; set; }
            public string CategoryName { get; set; }
        }
    }
}
