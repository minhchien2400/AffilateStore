using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Common.Service;
using AffiliateStoreBE.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using Volo.Abp;
using DocumentFormat.OpenXml.Spreadsheet;
using AffiliateStoreBE.Controllers;
using Aspose.Cells;
using Worksheet = Aspose.Cells.Worksheet;

namespace AffiliateStoreBE.Service
{
    public class ImportProductsService
    {
        private string _loggerSuffix = string.Empty;
        private decimal _miniProgressBar = 0.89m;
        private decimal _miniUploadStreamProgressBar = 0.95m;
        //private readonly ICustomDistributedCache _distributedCache;
        private Guid _currentUserId;
        private string sheetName_Candidates;
        private bool enableSemester = false;
        private int _totalDataRow = 0;
        private int _currentDataRow = 0;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private List<ProductSheetModel> _excelProducts;
        private List<ImageSheetModel> _excelImages;
        private string sheetName_Product = "";
        private string sheetName_Image = "";
        private readonly ExcelHelper _excelHelper;
        public ImportProductsService(IHttpContextAccessor httpContextAccessor, ExcelHelper excelHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _excelHelper = excelHelper;
        }

        public async Task<bool> ImportProductExcel(ImportPathInfo request)
        {
            try
            {
               // validate here

                var originalSheets = new Dictionary<string, Worksheet>();

                using (var fs = new MemoryStream(request.ImportFileBytes))
                {
                    ReadExcel(fs, request.Language);

                    if (_excelProducts == null || _excelImages == null)
                    {
                        return false;
                    }

                    await GenerateReport(workbook, report, originalSheets, request.Language);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ValidateFileFormat(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (!".xlsx".EqualsIgnoreCase(extension))
            {
                return false;
            }
            return true;
        }

        private void ReadExcel(Stream stream, String language)
        {
            var productsExcel = ExcelHelper.ReadExcel<ProductSheetModel>(stream, sheetName_Product);
            var imagesExcel = ExcelHelper.ReadExcel<ImageSheetModel>(stream, sheetName_Image);
            var productItem = new ProductSheetModel();
            var imageItem = new ImageSheetModel();
            foreach(var pr in productsExcel)
            {
                productItem.ProductName = pr["Name"];
                productItem.Description = pr["Description"];
                productItem.Price = int.Parse(pr["Price"]);
                productItem.CategoryName = pr["Category"];
                _excelProducts.Add(productItem);
            }
            foreach (var im in imagesExcel)
            {
                imageItem.ProductName = im["Name"];
                imageItem.Image = im["Image"];
                _excelProducts.Add(productItem);
            }
        }
        
        private void UpdateProductInfo()
        {
             
        }
       
        private async Task GenerateReport(Workbook workbook, ImportReportInfo report, Dictionary<string, Worksheet> originalSheets, String language)
        {
            ExcelLanguageType excellanguage = ExcelLanguageType.English;
            var languageTemplateName = String.Empty;
            //Switch Japanese Template if Japanese
            if (!String.IsNullOrEmpty(language) && language.IndexOf("ja-jp", StringComparison.OrdinalIgnoreCase) > -1)
            {
                languageTemplateName = "_jp";
                excellanguage = ExcelLanguageType.Japanese;
            }
            //Switch German Template if German
            else if (!String.IsNullOrEmpty(language) && language.IndexOf("de-de", StringComparison.OrdinalIgnoreCase) > -1)
            {
                languageTemplateName = "_de";
                excellanguage = ExcelLanguageType.German;
            }
            //Switch Chinese Template if Chinese
            else if (!String.IsNullOrEmpty(language) && language.IndexOf("zh-cn", StringComparison.OrdinalIgnoreCase) > -1)
            {
                languageTemplateName = "_cn";
                excellanguage = ExcelLanguageType.Chinese;
            }
            //Switch Dutch Template if Dutch
            else if (!String.IsNullOrEmpty(language) && language.IndexOf("nl-nl", StringComparison.OrdinalIgnoreCase) > -1)
            {
                languageTemplateName = "_nl";
                excellanguage = ExcelLanguageType.Dutch;
            }

            var courseTemplate = String.Format(Constants.CourseTemplate, languageTemplateName);

            foreach (var sheet in originalSheets)
            {
                if (sheet.Key.EqualsIgnoreCase(sheetName_Course))
                {
                    ExcelHelper.UpdateWorksheetWithReport(sheet.Value, _excelCourse, excellanguage);
                }
                if (sheet.Key.EqualsIgnoreCase(sheetName_Classes))
                {
                    ExcelHelper.UpdateWorksheetWithReport(sheet.Value, _excelClasses, excellanguage);
                }
                if (sheet.Key.EqualsIgnoreCase(sheetName_Candidates))
                {
                    ExcelHelper.UpdateWorksheetWithReport(sheet.Value, _excelCandidates, excellanguage);
                }
            }

            var allRow = new List<ExcelDynamic>();
            allRow.AddRange(_excelCourse);
            allRow.AddRange(_excelClasses);
            allRow.AddRange(_excelCandidates);

            if (allRow.All(a => a.ReportStatus == ImportStatus.Skip))
            {
                report.Status = ImportReportStatus.Skip;
            }
            else if (allRow.All(a => a.ReportStatus == ImportStatus.Failed))
            {
                report.Status = ImportReportStatus.Failed;
            }
            else if (!allRow.Any(a => a.ReportStatus == ImportStatus.Failed))
            {
                report.Status = ImportReportStatus.Success;
            }
            else if (allRow.Any(a => a.ReportStatus == ImportStatus.Failed))
            {
                report.Status = ImportReportStatus.PartialFailed;
            }

            var newStream = new MemoryStream();
            workbook.Save(newStream, SaveFormat.Xlsx);
            report.ReportBytes = newStream.ToArray();
        }

    public class ProductSheetModel
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string CategoryName { get; set; }
    }

    public class ImageSheetModel
    {
        public string ProductName { get; set; }
        public string Image { get; set; }
    }
}
