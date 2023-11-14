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
        public async Task<IActionResult> GetDataHomePage(string stringInput)
        {
            try
            {
                var listStringSearch = _searchStringFunction.SearchString(stringInput);
                var products = await _storeDbContext.Set<Product>().Where(a => a.Status == Status.Active).ToListAsync();
                var productsDetail = products.Select(a => new ProductSearch { Id = a.Id, Name = a.Name }).ToList();
                productsDetail.ForEach(t => { t.Name = _searchStringFunction.RemoveSpaceAndConvert(t.Name); });
                var listProductIds = new List<Guid>();
                foreach (var stringSearch in listStringSearch)
                {
                    foreach(var product in productsDetail)
                    {
                        if(product.Name.ToLower().Contains(stringSearch.ToLower()))
                        {
                            listProductIds.Add(product.Id);
                        }
                    }
                }
                var productsSearch = products.Where(a => listProductIds.Contains(a.Id)).Distinct().ToList();
                return Ok(productsSearch);
            }
            catch(Exception e)
            {
                throw;
            }
        }
        static string RemoveAccents(string input)
        {
            string decomposed = input.Normalize(NormalizationForm.FormD);
            return Regex.Replace(decomposed, @"\p{Mn}", string.Empty);
        }

        public class ProductSearch
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}
