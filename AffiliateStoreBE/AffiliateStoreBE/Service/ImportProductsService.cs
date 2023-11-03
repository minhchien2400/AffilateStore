using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Common.Service;
using AffiliateStoreBE.Models;
using Worksheet = Aspose.Cells.Worksheet;
using AffiliateStoreBE.DbConnect;
using Microsoft.EntityFrameworkCore;
using AffiliateStoreBE.Controllers;
using static AffiliateStoreBE.Service.ImportProductsService;

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
        private List<ProductDetailModel> _productsDetail;
        private string sheetName_Product = "";
        private string sheetName_Image = "";
        private readonly IProductsService _productsService;
        private readonly StoreDbContext _storeDbContext;
        private readonly ICategoryService _categoryService;

        public ImportProductsService(StoreDbContext storeDbContext, IHttpContextAccessor httpContextAccessor, IProductsService productsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _productsService = productsService;
            _storeDbContext = storeDbContext;
        }

        public async Task<bool> ImportProductExcel(ImportPathInfo request)
        {
            try
            {
                // validate here

                var originalSheets = new Dictionary<string, Worksheet>();

                using (var fs = new MemoryStream(request.ImportFileBytes))
                {
                    ReadExcel(fs);

                    // await GenerateReport(Workbook, Report, originalSheets, request.Language);
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

        private void ReadExcel(Stream stream)
        {
            var productsExcel = ExcelHelper.ReadExcel<ProductSheetModel>(stream, sheetName_Product);
            var imagesExcel = ExcelHelper.ReadExcel<ImageSheetModel>(stream, sheetName_Image);
            var productDetail = new ProductDetailModel();
            foreach (var pr in productsExcel)
            {
                productDetail.ProductName = pr["Name"];
                productDetail.Description = pr["Description"];
                productDetail.Price = int.Parse(pr["Price"]);
                productDetail.CategoryName = pr["Category"];
                productDetail.Images.AddRange(imagesExcel.SelectMany(dict => dict.Where(entry => entry.Key.ToLower().Equals(pr["Name"])).Select(entry => entry.Value).ToList()));
                _productsDetail.Add(productDetail);
            }
        }

        private async Task<bool> UpdateProductInfo()
        {
            var productsInDb = await _storeDbContext.Set<Product>().Where(a => a.IsActive && _productsDetail.Select(e => e.ProductName).ToList().Contains(a.Name)).ToListAsync();
            var productsUpdate = _productsDetail.Where(a => productsInDb.Select(p => p.Name.ToLower()).Equals(a.ProductName)).ToList();
            var productsCreate = _productsDetail.Except(productsUpdate).ToList();
            var categorys = await _categoryService.GetCategoryByName(productsCreate.Select(p => p.ProductName).ToList());
            foreach (var productDb in productsInDb)
            {
                var productUpdate = productsUpdate.Where(a => a.ProductName.ToLower().Equals(productDb.Name)).FirstOrDefault();
                productDb.Description = productUpdate.Description;
                productDb.Price = productUpdate.Price;
                productDb.Images = String.Join(", ", productUpdate.Images);
            }
            var productsCreateDb = productsCreate.Select(a => new Product
            {
                Id = Guid.NewGuid(),
                Name = a.ProductName,
                Description = a.Description,
                Price = a.Price,
                Images = String.Join(", ", a.Images),
                CategoryId = categorys.Where(c => c.Name.ToLower().Equals(a.ProductName.ToLower())).Select(c => c.Id).FirstOrDefault()
            }) ;
            
            return true;
        }

        //private async Task GenerateReport(Workbook workbook, ImportReportInfo report, Dictionary<string, Worksheet> originalSheets, String language)
        //{
        //    ExcelLanguageType excellanguage = ExcelLanguageType.English;
        //    var languageTemplateName = String.Empty;
        //    //Switch Japanese Template if Japanese
        //    if (!String.IsNullOrEmpty(language) && language.IndexOf("ja-jp", StringComparison.OrdinalIgnoreCase) > -1)
        //    {
        //        languageTemplateName = "_jp";
        //        excellanguage = ExcelLanguageType.Japanese;
        //    }
        //    //Switch German Template if German
        //    else if (!String.IsNullOrEmpty(language) && language.IndexOf("de-de", StringComparison.OrdinalIgnoreCase) > -1)
        //    {
        //        languageTemplateName = "_de";
        //        excellanguage = ExcelLanguageType.German;
        //    }
        //    //Switch Chinese Template if Chinese
        //    else if (!String.IsNullOrEmpty(language) && language.IndexOf("zh-cn", StringComparison.OrdinalIgnoreCase) > -1)
        //    {
        //        languageTemplateName = "_cn";
        //        excellanguage = ExcelLanguageType.Chinese;
        //    }
        //    //Switch Dutch Template if Dutch
        //    else if (!String.IsNullOrEmpty(language) && language.IndexOf("nl-nl", StringComparison.OrdinalIgnoreCase) > -1)
        //    {
        //        languageTemplateName = "_nl";
        //        excellanguage = ExcelLanguageType.Dutch;
        //    }

        //    var courseTemplate = String.Format(Constants.CourseTemplate, languageTemplateName);

        //    foreach (var sheet in originalSheets)
        //    {
        //        if (sheet.Key.EqualsIgnoreCase(sheetName_Course))
        //        {
        //            ExcelHelper.UpdateWorksheetWithReport(sheet.Value, _excelCourse, excellanguage);
        //        }
        //        if (sheet.Key.EqualsIgnoreCase(sheetName_Classes))
        //        {
        //            ExcelHelper.UpdateWorksheetWithReport(sheet.Value, _excelClasses, excellanguage);
        //        }
        //        if (sheet.Key.EqualsIgnoreCase(sheetName_Candidates))
        //        {
        //            ExcelHelper.UpdateWorksheetWithReport(sheet.Value, _excelCandidates, excellanguage);
        //        }
        //    }

        //    var allRow = new List<ExcelDynamic>();
        //    allRow.AddRange(_excelCourse);
        //    allRow.AddRange(_excelClasses);
        //    allRow.AddRange(_excelCandidates);

        //    if (allRow.All(a => a.ReportStatus == ImportStatus.Skip))
        //    {
        //        report.Status = ImportReportStatus.Skip;
        //    }
        //    else if (allRow.All(a => a.ReportStatus == ImportStatus.Failed))
        //    {
        //        report.Status = ImportReportStatus.Failed;
        //    }
        //    else if (!allRow.Any(a => a.ReportStatus == ImportStatus.Failed))
        //    {
        //        report.Status = ImportReportStatus.Success;
        //    }
        //    else if (allRow.Any(a => a.ReportStatus == ImportStatus.Failed))
        //    {
        //        report.Status = ImportReportStatus.PartialFailed;
        //    }

        //    var newStream = new MemoryStream();
        //    workbook.Save(newStream, SaveFormat.Xlsx);
        //    report.ReportBytes = newStream.ToArray();
        //}

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

        public class ProductDetailModel
        {
            public string ProductName { get; set; }
            public string Description { get; set; }
            public int Price { get; set; }
            public string CategoryName { get; set; }
            public List<string> Images { get; set; }
        }
    }
}
