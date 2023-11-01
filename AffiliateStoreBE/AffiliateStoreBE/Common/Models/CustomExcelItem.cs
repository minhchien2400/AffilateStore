namespace AffiliateStoreBE.Common.Models
{
    public class CustomExcelItem
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public object Value { get; set; }
        public ExcelItemStyle ExcelItemStyle { get; set; }
    }
    public enum ExcelItemStyle
    {
        None = 0,
        ExcelNotes = 1,
        FlagHighLight = 2,
        VarianceHighLight = 3
    }
}
