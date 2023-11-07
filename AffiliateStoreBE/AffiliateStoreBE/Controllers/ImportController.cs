using AffiliateStoreBE.Common;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using AffiliateStoreBE.Service;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp;
using static AffiliateStoreBE.Common.Models.ImportModel;

namespace AffiliateStoreBE.Controllers
{
    public class ImportController : ControllerBase
    {
        private readonly ImportProductsService _importProductsService;
        private readonly ImportCategoryService _importCategoryService;
        private readonly ImportVideoReviewService _importVideoReviewService;
        public ImportController(StoreDbContext storeDbContext, ImportProductsService importProductsService, ImportCategoryService importCategoryService, ImportVideoReviewService importVideoReviewService)
        {
            _importProductsService = importProductsService;
            _importCategoryService = importCategoryService;
            _importVideoReviewService = importVideoReviewService;
        }

        [HttpPost("importproducts")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ImportProducts([FromForm] ImportRequest command)
        {
            var file = HttpContext.Request.Form.Files[0];
            command.ImportFile = file;
            try
            {
                var item = Request.Cookies.FirstOrDefault(a => a.Key == CookieRequestCultureProvider.DefaultCookieName);
                command.Language = item.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
            var importFileBytes = UploaderHelper.GetBytes(file);
            var fileImportInfo = new ImportPathInfo()
            {
                FileName = command.ImportFile.FileName,
                ImportFileBytes = importFileBytes,
                Language = command.Language,
            };
            await _importProductsService.ImportProductExcel(fileImportInfo);
            return Ok();
        }

        [HttpPost("importcategorys")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ImportCategorys([FromForm] ImportRequest command)
        {

            var file = HttpContext.Request.Form.Files[0];
            command.ImportFile = file;
            try
            {
                var item = Request.Cookies.FirstOrDefault(a => a.Key == CookieRequestCultureProvider.DefaultCookieName);
                command.Language = item.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
            var importFileBytes = UploaderHelper.GetBytes(file);
            var fileImportInfo = new ImportPathInfo()
            {
                FileName = command.ImportFile.FileName,
                ImportFileBytes = importFileBytes,
                Language = command.Language,
            };
            await _importCategoryService.ImportCategoryExcel(fileImportInfo);
            return Ok();
        }

        [HttpPost("importvideoreview")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ImportVideosReview([FromForm] ImportVideoReviewRequest command)
        {

            var file = HttpContext.Request.Form.Files[0];
            command.ImportFile = file;
            try
            {
                var item = Request.Cookies.FirstOrDefault(a => a.Key == CookieRequestCultureProvider.DefaultCookieName);
                command.Language = item.Value;
            }
            catch (Exception ex)
            {
                throw;
            }
            var importFileBytes = UploaderHelper.GetBytes(file);
            var fileImportInfo = new ImportVideoReviewPathInfo()
            {
                FileName = command.ImportFile.FileName,
                ImportFileBytes = importFileBytes,
                Language = command.Language,
                ProductId = command.ProductId,
            };
            await _importVideoReviewService.ImportVideoReviewExcel(fileImportInfo);
            return Ok();
        }

        public class ImportVideoReviewRequest : ImportRequest
        {
            public Guid ProductId { get; set; }
        }

        public class ImportVideoReviewPathInfo : ImportPathInfo
        {
            public Guid ProductId { get; set;}
        }
    }
}
