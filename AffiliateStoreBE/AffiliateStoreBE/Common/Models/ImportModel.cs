using System.ComponentModel;

namespace AffiliateStoreBE.Common.Models
{
    public class ImportModel
    {
        public class ImportRequest
        {
            public IFormFile ImportFile { get; set; }

            public String Language { get; set; }
        }

        public class ImportPathInfo
        {
            public string FileName { get; set; }
            public byte[] ImportFileBytes { get; set; }
            public string Language { get; set; }
        }

        public class CategorySheetModel
        {
            [ExcelColumn("Name")]
            public string CategoryName { get; set; }
            [ExcelColumn("Image")]
            public string Image { get; set; }
        }

        public class ProductSheetModel
        {
            [ExcelColumn("Name")]
            public string ProductName { get; set; }
            [ExcelColumn("Description")]
            public string Description { get; set; }
            [ExcelColumn("Cost")]
            public float Cost { get; set; }
            [ExcelColumn("Price")]
            public float Price { get; set; }
            [ExcelColumn("Category name")]
            public string CategoryName { get; set; }
            [ExcelColumn("Stars")]
            public int Stars { get; set; }
            [ExcelColumn("Affiliate link")]
            public string AffLink { get; set; }
            //[ExcelColumn("Total sales")]
            //public int TotalSales { get; set; }
        }

        public class ImageSheetModel
        {
            [ExcelColumn("Product name")]
            public string ProductName { get; set; }
            [ExcelColumn("Image link")]
            public string Image { get; set; }
        }

        public class VideoReivewSheetModel
        {
            [ExcelColumn("Title")]
            public string Title { get; set; }
            [ExcelColumn("Description")]
            public string Description { get; set; }
            [ExcelColumn("Video link")]
            public string VideoLink { get; set; }
        }

        public enum ImportStatus
        {
            [Description("")]
            NA = 0,

            [Description("")]
            Success = 1,

            [Description("")]
            Failed = 2,

            [Description("")]
            Skip = 3,

            [Description("")]
            Ignore = 4,
        }
    }
}
