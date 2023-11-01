using LinqToExcel.Attributes;
using System.ComponentModel;

namespace AffiliateStoreBE.Common.Models
{
    public class ExcelDynamic
    {
        [ExcelColumn("FeatureCommon_Export_Common_ImportStatus_Entry")]
        public ImportStatus ReportStatus { get; set; } = ImportStatus.NA;

       //[ExcelColumn("FeatureCommon_Export_Common_Comments_Entry", false, true)]
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

        //private Dictionary<string, object> fields = new Dictionary<string, object>();
        //public bool IsMustValidatePass { get; set; } = true;
        //public bool IsDuplicate { get; set; } = false;
        //public bool IsScopePass { get; set; } = true;
        //public bool HasPermission { get; set; } = true;
        //public bool IsStatusPass { get; set; } = true;
        public bool HasChanged { get; set; } = false;

        //public bool IsInvalid { get; set; } = false;
        public int RowIndex { get; set; } = -1;

        //public void SetProperty(string name, object value)
        //{
        //    if (fields.ContainsKey(name))
        //    {
        //        fields.Remove(name);
        //    }
        //    fields.Add(name, value);
        //}

        //public object GetProperty(string name)
        //{
        //    return fields.ContainsKey(name) ? fields[name] : null;
        //}

        //public Dictionary<string, object> GetAllProperties()
        //{
        //    return fields;
        //}

        //public bool IsExist(string name)
        //{
        //    return fields.ContainsKey(name);
        //}
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
}
