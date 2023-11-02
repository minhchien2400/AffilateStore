using LinqToExcel.Attributes;
using System.ComponentModel;

namespace AffiliateStoreBE.Common.Models
{
    public class ExcelDynamic
    {
        public ImportStatus ReportStatus { get; set; } = ImportStatus.NA;

        public string ReportMessage
        {
            get
            {
                if (ReportStatus == ImportStatus.Failed && InvalidMessage.Any())
                {
                    return string.Join(Environment.NewLine, InvalidMessage);
                }
                else if (ReportStatus == ImportStatus.Skip)
                {
                    return SkipMessage;
                }
                else if (ReportStatus == ImportStatus.Success)
                {
                    return SuccessMessage;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public List<string> InvalidMessage { get; set; } = new List<string>();
        public string SkipMessage { get; set; }
        public string SuccessMessage { get; set; }
        public bool HasChanged { get; set; } = false;

        public int RowIndex { get; set; } = -1;
    }
    public class DynamicExcelModel
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public enum ImportStatus
    {
        [Description("")]
        NA = 0,

        [Description("FeatureCommon_Export_Common_Successful_Entry")]
        Success = 1,

        [Description("FeatureCommon_Export_Common_Failed_Entry")]
        Failed = 2,

        [Description("FeatureCommon_Export_Common_Skip_Entry")]
        Skip = 3,

        [Description("")]
        Ignore = 4,
    }
    public enum ImportType
    {
        ImportProducts = 0,
        ImportImages = 1,
        ImportCategorys = 2,
    }
}
