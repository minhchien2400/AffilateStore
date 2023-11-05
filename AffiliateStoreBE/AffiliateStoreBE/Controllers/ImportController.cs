using AffiliateStoreBE.Common.Service;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using AffiliateStoreBE.Service;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp;

namespace AffiliateStoreBE.Controllers
{
    public class ImportController : ControllerBase
    {
        private readonly ImportProductsService _importProductsService;
        private readonly ImportCategoryService _importCategoryService;
        public ImportController(StoreDbContext storeDbContext, ImportProductsService importProductsService, ImportCategoryService importCategoryService)
        {
            _importProductsService = importProductsService;
            _importCategoryService = importCategoryService;
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
    }
}
