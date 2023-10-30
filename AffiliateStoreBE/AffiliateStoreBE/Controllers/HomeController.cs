using AffiliateStoreBE.DbConnect;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AffiliateStoreBE.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly StoreDbContext storeDbContext;
        public HomeController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        [HttpGet("homepage")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetDataHomePage()
        {
            try
            { }
            catch(Exception e)
            {
                throw;
            }
            return Ok(true);
        }
    }
}
