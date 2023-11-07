using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Common;
using AffiliateStoreBE.DbConnect;
using AffiliateStoreBE.Models;
using Aspose.Cells;
using Microsoft.EntityFrameworkCore;
using static AffiliateStoreBE.Controllers.ImportController;
using System.Linq;
using AffiliateStoreBE.Service.IService;
using static AffiliateStoreBE.Common.Models.ImportModel;

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
        public async Task ImportVideoReviewExcel(ImportVideoReviewPathInfo request)
        {
            try
            {
                // validate here

                var originalSheets = new Dictionary<string, Worksheet>();

                using (var fs = new MemoryStream(request.ImportFileBytes))
                {
                    var videosReviewExcels = ReadExcel(fs);
                    await InitDatas(videosReviewExcels, request.ProductId);
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
            var productsExcel = ExcelHelper.ReadExcel<VideoReivewSheetModel>(stream, sheetName_Video);
            return productsExcel;
        }

        private async Task InitDatas(List<Dictionary<string, string>> excelVideosReview, Guid productId)
        {
            var videosReviewsDb = await _storeDbContext.Set<VideoReview>().Where(a => a.ProductId == productId).ToListAsync();
            var videosReviewUpdate = excelVideosReview.Where(a => videosReviewsDb.Select(x => x.VideoLink).ToList().Contains(a["Video link"])).ToList();
            var videosReviewAdd = excelVideosReview.Except(videosReviewUpdate).ToList();
            var productsDb = await _productsService.GetProductsByIds(videosReviewsDb.Select(a => a.ProductId).ToList());
            if (videosReviewsDb.Any())
            {
                foreach (var videoReview in videosReviewsDb)
                {
                    var videoUpdate = videosReviewUpdate.Where(a => a.Select(x => x.Value).ToList().Contains(videoReview.Title) || a.Select(x => x.Value).ToList().Contains(videoReview.Description) || a.Select(x => x.Value).ToList().Contains(videoReview.VideoLink)).FirstOrDefault();
                    videoReview.Title = videoUpdate["Title"];
                    videoReview.Description = videoUpdate["Description"];
                    videoReview.VideoLink = videoUpdate["Video link"];
                }
            }
            if (videosReviewAdd.Any())
            {
                var newListVideos = new List<VideoReview>();
                foreach (var videoAdd in videosReviewAdd)
                {
                    var newVideo = new VideoReview();
                    newVideo.Id = Guid.NewGuid();
                    newVideo.Title = videoAdd["Title"];
                    newVideo.Description = videoAdd["Description"];
                    newVideo.VideoLink = videoAdd["Video link"];
                    newListVideos.Add(newVideo);
                }
                await _storeDbContext.AddRangeAsync(newListVideos);
            }
            await _storeDbContext.SaveChangesAsync();
        }

        
        public class VideoReviewExcel
        {
            public string Title { get; set; }
            public string VideoLink { get; set; }
        }
    }
}
