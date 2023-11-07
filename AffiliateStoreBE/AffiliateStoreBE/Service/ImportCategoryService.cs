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
            var (categoriesUpdate, categoriesCreate) = await CheckCategorysExcel(excelsCategory.Distinct().ToList());
            if (categoriesUpdate.Any())
            {
                foreach (var category in categoriesUpdate)
                {
                    var categoryUpdateImage = categoriesUpdate.Where(a => a["Name"].ToLower().Equals(category.Name.ToLower())).FirstOrDefault();
                    if (categoryUpdateImage != null)
                    {
                        category.Image = categoryUpdateImage["Image"];
                    }
                    else
                    {
                        category.Name = categoriesUpdate.Where(a => a["Image"].Equals(category.Image)).Select(a => a["Image"]).FirstOrDefault();
                    }
                }
            }
            if (categorisCreate.Any())
            {
                var newListCategory = new List<Category>();
                foreach (var category in categorisCreate)
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

        private async Task<(List<Category>, List<Dictionary<string, string>>, List<Dictionary<string, string>>)> CheckCategorysExcel(List<Dictionary<string, string>> excelsCategory)
        {
            var categoriesNameExcel = excelsCategory.Select(a => a["Name"].ToLower()).ToList();
            var imagesExcel = excelsCategory.Select(a => a["Image"].ToLower()).ToList();
            var categoriesDb = await _storeDbContext.Set<Category>().Where(a => a.Status == Status.Active && (categoriesNameExcel.Contains(a.Name.ToLower()) || (imagesExcel.Contains(a.Image)))).ToListAsync();
            var categoriesExist = categoriesDb.Where(a => categoriesNameExcel.Contains(a.Name.ToLower()) && imagesExcel.Contains(a.Image)).ToList();
            var categoriesCreate = new List<Dictionary<string, string>>();
            foreach(var category in categoriesDb)
            {
                categoriesCreate.AddRange(excelsCategory.Where(a => !a["Name"].ToLower().Equals(category.Name.ToLower()) && !a["Image"].Equals(category.Image)).Distinct().ToList());
            }
            var categoriesUpdate = categoriesDb.Except(categoriesExist).ToList();
            return (categoriesUpdate, categoriesCreate);
        }
    }
}
