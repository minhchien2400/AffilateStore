using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace AffiliateStoreBE.Controllers
{
    public class VideoReviewController : ControllerBase
    {
        private readonly StoreDbContext _storeDbContext;
        public VideoReviewController(StoreDbContext storeDbContext)
        { 
            _storeDbContext = storeDbContext;
        }

        [HttpGet("getvideoreview")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetVideosReview(Guid productId)
        {
            try
            {
                var videos = await _storeDbContext.Set<VideoReview>().Where(a => a.ProductId == productId && a.Status == Status.Active).Select(a => new 
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.VideoLink,
                    a.ProductId,
                }).ToListAsync();
                return Ok(videos);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
