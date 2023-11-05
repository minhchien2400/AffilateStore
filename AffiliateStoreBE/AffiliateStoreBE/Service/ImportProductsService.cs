using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Common.Service;
using AffiliateStoreBE.Models;
using Worksheet = Aspose.Cells.Worksheet;
using AffiliateStoreBE.DbConnect;
using Microsoft.EntityFrameworkCore;
using AffiliateStoreBE.Controllers;
using static AffiliateStoreBE.Service.ImportProductsService;
using LinqToExcel.Extensions;

namespace AffiliateStoreBE.Service
{
    public class ImportProductsService
    {
        private string sheetName_Product = "Products";
        private string sheetName_Image = "Images";
        private readonly StoreDbContext _storeDbContext;
        private readonly ICategoryService _categoryService;

        public ImportProductsService(StoreDbContext storeDbContext, ICategoryService categoryService)
        {
            _storeDbContext = storeDbContext;
            _categoryService = categoryService;
        }

        public async Task ImportProductExcel(ImportPathInfo request)
        {
            try
            {
                // validate here

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
                productDetail.Price = int.Parse(pr["Price"]);
                productDetail.CategoryName = pr["Category"];
                foreach(var imageExcel in imagesExcel)
                {
                    if (imageExcel.Any(i => i.Key.Equals("Product name") && i.Value.ToLower().Equals(pr["Name"].ToLower())))
                    {
                        images.Add(imageExcel["Image"]);
                    }
                }
                productDetail.Image = String.Join("; ", images);
                productsDetail.Add(productDetail);
            }
            return productsDetail;
        }

        private async Task InitDatas(List<ProductDetailModel> productsDetail)
        {
            var productsInDb = await _storeDbContext.Set<Product>().Where(a => a.IsActive && productsDetail.Select(e => e.ProductName).ToList().Contains(a.Name)).ToListAsync();
            var productsUpdate = new List<ProductDetailModel>();
            if(productsInDb.Any())
            {
                productsUpdate = productsDetail.Where(a => productsInDb.Select(p => p.Name.ToLower()).Equals(a.ProductName)).ToList();
                foreach (var productDb in productsInDb)
                {
                    var productUpdate = productsUpdate.Where(a => a.ProductName.ToLower().Equals(productDb.Name)).FirstOrDefault();
                    productDb.Description = productUpdate.Description;
                    productDb.Price = productUpdate.Price;
                    productDb.Images = productUpdate.Image;
                }
            }
            var productsCreate = productsDetail.Except(productsUpdate).ToList();
            if(productsCreate.Any())
            {
                var categorys = await _categoryService.GetCategoryByName(productsCreate.Select(p => p.CategoryName).ToList());
                var productsCreateDb = productsCreate.Select(a => new Product
                {
                    Id = Guid.NewGuid(),
                    Name = a.ProductName,
                    Description = a.Description,
                    Price = a.Price,
                    Images = a.Image,
                    CategoryId = categorys.Where(c => c.Name.ToLower().Equals(a.CategoryName.ToLower())).Select(c => c.Id).FirstOrDefault()
                }).ToList();
                await _storeDbContext.AddRangeAsync(productsCreateDb);
            }
            await _storeDbContext.SaveChangesAsync();
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
            [ExcelColumn("Name")]
            public string ProductName { get; set; }
            [ExcelColumn("Description")]
            public string Description { get; set; }
            [ExcelColumn("Price")]
            public int Price { get; set; }
            [ExcelColumn("Category")]
            public string CategoryName { get; set; }
        }

        public class ImageSheetModel
        {
            [ExcelColumn("Product name")]
            public string ProductName { get; set; }
            [ExcelColumn("Image")]
            public string Image { get; set; }
        }

        public class ProductDetailModel
        {
            public string ProductName { get; set; }
            public string Description { get; set; }
            public int Price { get; set; }
            public string CategoryName { get; set; }
            public string Image { get; set; }
        }
    }
}
