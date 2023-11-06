using AffiliateStoreBE.Common;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Aspose.Cells;
using LinqToExcel.Attributes;

namespace AffiliateStoreBE.Service
{
    public class ImportCategoryService
    {
        private readonly StoreDbContext _storeDbContext;
        private readonly ICategoryService _categoryService;
        private string sheetName_Category = "Category";
        public ImportCategoryService(StoreDbContext storeDbContext, ICategoryService categoryService)
        {
            _storeDbContext = storeDbContext;
            _categoryService = categoryService;
        }
        public async Task ImportCategoryExcel(ImportPathInfo request)
        {
            try
            {
                // validate here

                var originalSheets = new Dictionary<string, Worksheet>();

                using (var fs = new MemoryStream(request.ImportFileBytes))
                {
                    var categoryExcel = ReadExcel(fs);
                    await InitDatas(categoryExcel);
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

        private List<Dictionary<string, string>> ReadExcel(Stream stream)
        {
            var productsExcel = ExcelHelper.ReadExcel<CategorySheetModel>(stream, sheetName_Category);
            return productsExcel;
        }

        private async Task InitDatas(List<Dictionary<string, string>> excelsCategory)
        {
            var categorysDb = await _categoryService.GetCategoryByName(excelsCategory.Select(e => e["Name"]).Distinct().ToList());
            var categorysCreate = excelsCategory.Where(a =>  !categorysDb.Select(c => c.Name).ToList().Contains(a["Name"])).ToList();
            if(categorysDb.Any())
            {
                foreach(var category in categorysDb)
                {
                    category.Image = excelsCategory.Where(a => a["Name"].ToLower() == category.Name.ToLower()).Select(a => a["Image"]).FirstOrDefault();
                }
            }
            if(categorysCreate.Any())
            {
                var newListCategory = new List<Category>();
                foreach(var category in categorysCreate)
                {
                    var newCategory = new Category();
                    newCategory.Id = Guid.NewGuid();
                    newCategory.Name = category["Name"];   
                    newCategory.Image = category["Image"];
                    newListCategory.Add(newCategory);
                }
                await _storeDbContext.AddRangeAsync(newListCategory);
            }
            await _storeDbContext.SaveChangesAsync();
        }
        public class CategorySheetModel
        {
            [ExcelColumn("Name")]
            public string CategoryName { get; set; }
            [ExcelColumn("Image")]
            public string Image { get; set; } 
        }
    }
}
