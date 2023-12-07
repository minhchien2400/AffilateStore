using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Common;
using AffiliateStoreBE.Models;
using Worksheet = Aspose.Cells.Worksheet;
using AffiliateStoreBE.DbConnect;
using Microsoft.EntityFrameworkCore;
using static AffiliateStoreBE.Common.Models.ImportModel;
using AffiliateStoreBE.Service.IService;
using ImportStatus = AffiliateStoreBE.Common.Models.ImportStatus;

namespace AffiliateStoreBE.Service
{
    public class ImportProductsService
    {
        private string sheetName_Product = "Product";
        private string sheetName_Image = "Image";
        private readonly StoreDbContext _storeDbContext;
        private readonly ICategoryService _categoryService;

        public ImportProductsService(StoreDbContext storeDbContext, ICategoryService categoryService)
        {
            _storeDbContext = storeDbContext;
            _categoryService = categoryService;
        }

        public async Task<ImportStatus> ImportProductExcel(ImportPathInfo request)
        {
            try
            {
                // validate here
                if(!ValidateFileFormat(request.FileName))
                {
                    return ImportStatus.Failed;    
                }

                var originalSheets = new Dictionary<string, Worksheet>();

                using (var fs = new MemoryStream(request.ImportFileBytes))
                {
                    var productsDetail = ReadExcel(fs);
                    await InitDatas(productsDetail);
                    // await GenerateReport(Workbook, Report, originalSheets, request.Language);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ImportStatus.Success;
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

        private List<ProductDetailModel> ReadExcel(Stream stream)
        {
            var productsDetail = new List<ProductDetailModel>();
            var productsExcel = ExcelHelper.ReadExcel<ProductSheetModel>(stream, sheetName_Product);
            var imagesExcel = ExcelHelper.ReadExcel<ImageSheetModel>(stream, sheetName_Image);
            foreach (var pr in productsExcel)
            {
                var productDetail = new ProductDetailModel();
                var images = new List<string>();
                productDetail.ProductName = pr["Name"];
                productDetail.Description = pr["Description"];
                productDetail.Cost = float.Parse(pr["Cost"]);
                productDetail.Price = float.Parse(pr["Price"]);
                productDetail.CategoryName = pr["Category name"];
                productDetail.Stars = int.Parse(pr["Stars"]);
                productDetail.AffLink = pr[("Affiliate link")];
                productDetail.TotalSales = int.Parse(pr[("Total sales")]);

                foreach (var imageExcel in imagesExcel)
                {
                    if (imageExcel.Any(i => i.Key.Equals("Product name") && i.Value.Equals(pr["Name"])))
                    {
                        images.Add(imageExcel["Image link"]);
                    }
                }
                productDetail.Image = String.Join(";", images);
                productsDetail.Add(productDetail);
            }
            return productsDetail;
        }

        private async Task InitDatas(List<ProductDetailModel> productsDetail)
        {
            try
            {
                var timeNow = DateTime.UtcNow;
                var productsInDb = await _storeDbContext.Set<Product>().Where(a => a.Status == Status.Active && productsDetail.Select(e => e.ProductName).Distinct().ToList().Contains(a.Name)).ToListAsync();
                var productsUpdate = new List<ProductDetailModel>();
                if (productsInDb.Any())
                {
                    productsUpdate = productsDetail.Where(a => productsInDb.Select(p => p.Name).Equals(a.ProductName)).ToList();
                    var categorys = await _categoryService.GetCategoryByName(productsUpdate.Select(p => p.CategoryName).ToList());
                    foreach (var productDb in productsInDb)
                    {
                        var productUpdate = productsUpdate.Where(a => a.ProductName.Equals(productDb.Name)).FirstOrDefault();
                        productDb.Description = productUpdate.Description;
                        productDb.Cost = productUpdate.Cost;
                        productDb.Price = productUpdate.Price;
                        productDb.Images = productUpdate.Image;
                        productDb.CategoryId = categorys.Where(c => c.Name.Equals(productUpdate.CategoryName)).Select(c => c.Id).FirstOrDefault();
                        productDb.Stars = productUpdate.Stars;
                        productDb.AffLink = productUpdate.AffLink;
                        productDb.TotalSales = productUpdate.TotalSales;
                        productDb.ModifiedTime = timeNow;
                    }
                }
                var productsCreate = productsDetail.Except(productsUpdate).ToList();
                if (productsCreate.Any())
                {
                    var categorys = await _categoryService.GetCategoryByName(productsCreate.Select(p => p.CategoryName).ToList());
                    var productsCreateDb = productsCreate.Select(a => new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = a.ProductName,
                        Description = a.Description,
                        Cost = a.Cost,
                        Price = a.Price,
                        Images = a.Image,
                        CategoryId = categorys.Where(c => c.Name.Equals(a.CategoryName)).Select(c => c.Id).FirstOrDefault(),
                        Stars = a.Stars,
                        AffLink = a.AffLink,
                        TotalSales = a.TotalSales,
                        CreatedTime = timeNow,
                    }).ToList();
                    await _storeDbContext.AddRangeAsync(productsCreateDb);
                }
                await _storeDbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            }
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

        public class ProductDetailModel
        {
            public string ProductName { get; set; }
            public string Description { get; set; }
            public float Cost { get; set; }
            public float Price { get; set; }
            public string CategoryName { get; set; }
            public int Stars { get; set; }
            public string AffLink { get; set; }
            public int TotalSales { get; set; }
            public string Image { get; set; }
        }
    }
}
