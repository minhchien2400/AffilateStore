using AffiliateStoreBE.Common;
using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using AffiliateStoreBE.Service.IService;
using Aspose.Cells;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.CompilerServices;
using static AffiliateStoreBE.Common.Models.ImportModel;

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
            var timeNow = DateTime.UtcNow;
            var (categoriesUpdate, categoriesExist) = await CheckCategorysExcel(excelsCategory.Distinct().ToList());
            excelsCategory = excelsCategory.Except(categoriesExist).ToList();
            var categoriesUpdateExcel = excelsCategory.Where(a => categoriesUpdate.Select(c => c.Name).Contains(a["Name"]) || categoriesUpdate.Select(c => c.Image).Contains(a["Image"])).ToList();
            var categoriesCreateExcel = excelsCategory.Except(categoriesUpdateExcel).ToList();
            {
                foreach (var category in categoriesUpdate)
                {
                    var categoryUpdateImage = categoriesUpdateExcel.Where(a => a["Name"].Equals(category.Name)).FirstOrDefault();
                    if (categoryUpdateImage != null)
                    {
                        category.Image = categoryUpdateImage["Image"];
                        category.ModifiedTime = timeNow;
                    }
                    else
                    {
                        category.Name = categoriesUpdateExcel.Where(a => a["Image"].Equals(category.Image)).Select(a => a["Name"]).FirstOrDefault();
                        category.ModifiedTime = timeNow;
                    }
                }
            }
            if (categoriesCreateExcel.Any())
            {
                var newListCategory = new List<Category>();
                foreach (var category in categoriesCreateExcel)
                {
                    var newCategory = new Category();
                    newCategory.Id = Guid.NewGuid();
                    newCategory.Name = category["Name"];
                    newCategory.Image = category["Image"];
                    newCategory.CreatedTime = timeNow;
                    newListCategory.Add(newCategory);
                }
                await _storeDbContext.AddRangeAsync(newListCategory);
            }
            await _storeDbContext.SaveChangesAsync();
        }

        private async Task<(List<Category>, List<Dictionary<string, string>>)> CheckCategorysExcel(List<Dictionary<string, string>> excelsCategory)
        {
            var categoriesNameExcel = excelsCategory.Select(a => a["Name"].ToLower()).ToList();
            var imagesExcel = excelsCategory.Select(a => a["Image"]).ToList();
            var categoriesDb = await _storeDbContext.Set<Category>().Where(a => a.Status == Status.Active && (categoriesNameExcel.Contains(a.Name.ToLower()) || (imagesExcel.Contains(a.Image)))).ToListAsync();
            var categoriesExist = categoriesDb.Where(a => categoriesNameExcel.Contains(a.Name.ToLower()) && imagesExcel.Contains(a.Image)).ToList();
            var categoriesDbFinal = categoriesDb.Except(categoriesExist).ToList();
            var categoriesExcelExist = new List<Dictionary<string, string>>();
            foreach (var category in categoriesExist)
            {
                categoriesExcelExist.AddRange(excelsCategory.Where(a => a["Name"].ToLower().Equals(category.Name.ToLower()) && a["Image"].Equals(category.Image)).Distinct().ToList());
            }
            return (categoriesDbFinal, categoriesExcelExist);
        }
    }
}
