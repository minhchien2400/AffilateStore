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
                var listProductsName = await _storeDbContext.Set<Product>().Where(a => a.Status == Status.Active).Select(a => a.Name.ToLower()).ToListAsync();
                return Ok(resultList);
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
    }
}
