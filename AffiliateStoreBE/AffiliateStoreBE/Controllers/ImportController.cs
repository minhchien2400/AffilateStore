using AffiliateStoreBE.Common.Service;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Volo.Abp;

namespace AffiliateStoreBE.Controllers
{
    public class ImportController : ControllerBase
    {
        private readonly StoreDbContext _storeDbContext;
        public ImportController(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }

        [HttpPost("importproducts")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ImportProduct([FromForm] ImportProductsRequest command)
        {
            try
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
                await _backgroundTaskService.Start<ImportCourseTaskAgent>(fileImportInfo, BackgroundTaskType.ImportCourseExcel, EventJobs.ImportCourse, typeof(AdminImportCourseEventContext), CommonConstants.Module.Admin,
                return Ok();
            }
            catch (BusinessException be)
            {
                var result = new
                {
                    HasError = true,
                    be.Message
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
