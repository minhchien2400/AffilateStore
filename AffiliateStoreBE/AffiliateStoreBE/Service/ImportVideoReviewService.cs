using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Common;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using DocumentFormat.OpenXml.Presentation;
using static AffiliateStoreBE.Service.ImportProductsService;
using System.Linq;
using Aspose.Cells;
using static AffiliateStoreBE.Service.ImportCategoryService;
using Microsoft.EntityFrameworkCore;
using LinqToExcel.Extensions;

namespace AffiliateStoreBE.Service
{
    public class ImportVideoReviewService
    {
        private string sheetName_Video = "Video";
        private readonly StoreDbContext _storeDbContext;
        private readonly IProductsService _productsService;
        public ImportVideoReviewService(StoreDbContext storeDbContext, IProductsService productsService)
        {
            _productsService = productsService;
            _storeDbContext = storeDbContext;
        }
        public async Task ImportVideoReviewExcel(ImportPathInfo request)
        {
            try
            {
                // validate here

                var originalSheets = new Dictionary<string, Worksheet>();

                using (var fs = new MemoryStream(request.ImportFileBytes))
                {
                    var videosReviewExcels = ReadExcel(fs);
                    await InitDatas(videosReviewExcels);
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
            var productsExcel = ExcelHelper.ReadExcel<CategorySheetModel>(stream, sheetName_Video);
            return productsExcel;
        }

        private async Task InitDatas(List<Dictionary<string, string>> excelVideosReview)
        {
            var videosReviewsDb = await CheckVideosReview(excelVideosReview);
            var productsDb = await _productsService.GetProductsByIds(videosReviewsDb.Select(a => a.ProductId).ToList());
            if (videosReviewsDb.Any())
            {
                foreach (var videoReview in videosReviewsDb)
                {
                    var videoReviewExcel = excelVideosReview.Where(a => a["Title"].ToLower().Equals(videoReview.Title.ToLower()) && a["Video link"].ToLower().Equals(videoReview.VideoLink)).FirstOrDefault();
                    if (videoReviewExcel != null && productsDb.Where(a => a.Name.ToLower().Equals(videoReviewExcel["Product name"].ToLower())).Select(a => a.ProductId).FirstOrDefault() != videoReview.ProductId)
                    {
                        videoReview.Id = productsDb.Where(a => a.Name.ToLower().Equals(videoReviewExcel["Product name"].ToLower())).Select(a => a.ProductId).FirstOrDefault();
                    }
                    videoReview.Description = videoReviewExcel["Description"]; 
                }
            }
            if (categorysCreate.Any())
            {
                var newListCategory = new List<Category>();
                foreach (var category in categorysCreate)
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

        private async Task<List<VideoReview>> CheckVideosReview(List<Dictionary<string, string>> excelVideosReview)
        {
            var videosReview = excelVideosReview.Select(e => new VideoReviewExcel { Title = e["Title"], VideoLink = e["Video link"] }).ToList();
            var videsReviewDb = await _storeDbContext.Set<VideoReview>().Where(a => videosReview.Select(v => v.Title.ToLower()).Contains(a.Title.ToLower()) && videosReview.Select(v => v.VideoLink.ToLower()).Contains(a.VideoLink.ToLower())).ToListAsync();
            return videsReviewDb;
        }
        public class VideoReivewSheetModel
        {
            [ExcelColumn("Product name")]
            public string ProductName { get; set; }
            [ExcelColumn("Title")]
            public string Title { get; set; }
            [ExcelColumn("Description")]
            public string Description { get; set; }
            [ExcelColumn("Video link")]
            public string VideoLink { get; set; }
        }
        public class VideoReviewExcel
        {
            public string Title { get; set; }
            public string VideoLink { get; set; }
        }
    }
}
