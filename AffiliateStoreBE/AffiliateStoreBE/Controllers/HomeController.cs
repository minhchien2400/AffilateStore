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
    public class HomeController : ControllerBase
    {
        private readonly StoreDbContext _storeDbContext;
        private readonly ISearchStringFunction _searchStringFunction;
        public HomeController(StoreDbContext storeDbContext, ISearchStringFunction searchStringFunction)
        {
            _storeDbContext = storeDbContext;
            _searchStringFunction = searchStringFunction;
        }

        [HttpGet("searchproducts")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetDataHomePage(SearchFilter request)
        {
            try
            {
                var listStringSearch = _searchStringFunction.SearchString(request.SearchText);
                var products = await _storeDbContext.Set<Product>().Where(a => a.Status == Status.Active).ToListAsync();
                if(request.CategoryName != string.Empty)
                {
                    products.Where(a => a.Category.Name.Equals(request.CategoryName));
                }
                var productsDetail = products.Select(a => new ProductSearch { Id = a.Id, Name = a.Name }).ToList();
                productsDetail.ForEach(t => { t.Name = _searchStringFunction.RemoveSpaceAndConvert(t.Name); });
                var listProductIds = new List<Guid>();
                foreach (var stringSearch in listStringSearch)
                {
                    foreach(var product in productsDetail)
                    {
                        if(product.Name.ToLower().Contains(stringSearch.ToLower()) && !listProductIds.Contains(product.Id))
                        {
                            listProductIds.Add(product.Id);
                        }
                    }
                }
                var productsSearch = products.Where(a => listProductIds.Contains(a.Id)).OrderBy(a => listProductIds.IndexOf(a.Id)).ToList();
                return Ok(productsSearch);
            }
            catch(Exception e)
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
