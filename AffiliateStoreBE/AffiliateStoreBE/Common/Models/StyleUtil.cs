using Aspose.Cells;

namespace AffiliateStoreBE.Common.Models
{
    public class StyleUtil
    {
        public Style EditableCellStyle { get; }
        public Style NormalCellStyle { get; }
        public Style TitleCellStyle { get; }
        public Style TextWrappedStyle { get; }
        public Style HighLightStyle { get; set; }
        public Style NoteCellStyle { get; set; }
        /// <summary>
        /// Setting Excel Styles
        /// </summary>
        /// <param name="language">Displays different font styles depending on the language</param>
        public StyleUtil(ExcelLanguageType language = ExcelLanguageType.English, bool needIsTextWrapped = false)
        {
            var fontName = String.Empty;
            switch (language)
            {
                case ExcelLanguageType.English:
                    fontName = "Calibri";
                    break;
                case ExcelLanguageType.Japanese:
                    fontName = "Meiryo UI";
                    break;
                case ExcelLanguageType.German:
                    fontName = "Calibri";
                    break;
                default:
                    fontName = "Calibri";
                    break;
            }

            var cellFactory = new CellsFactory();

            #region -- EditableCellStyle --

            EditableCellStyle = cellFactory.CreateStyle();
            //EditableCellStyle.ForegroundColor = System.Drawing.Color.FromArgb(198, 234, 206);
            EditableCellStyle.ForegroundColor = System.Drawing.Color.FromArgb(219, 237, 241);
            EditableCellStyle.Pattern = BackgroundType.Solid;
            EditableCellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            EditableCellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            EditableCellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            EditableCellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            EditableCellStyle.Font.Size = 11;
            EditableCellStyle.Font.Name = fontName;
            EditableCellStyle.Font.IsBold = false;
            EditableCellStyle.HorizontalAlignment = TextAlignmentType.Left;
            EditableCellStyle.VerticalAlignment = TextAlignmentType.Center;
            EditableCellStyle.IsTextWrapped = needIsTextWrapped;

            #endregion -- EditableCellStyle --

            #region -- NormalCellStyle --

            NormalCellStyle = cellFactory.CreateStyle();
            NormalCellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            NormalCellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            NormalCellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            NormalCellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            NormalCellStyle.Font.Size = 11;
            NormalCellStyle.Font.Name = fontName;
            NormalCellStyle.Font.IsBold = false;
            NormalCellStyle.HorizontalAlignment = TextAlignmentType.Left;
            NormalCellStyle.VerticalAlignment = TextAlignmentType.Center;
            NormalCellStyle.IsTextWrapped = needIsTextWrapped;

            #endregion -- NormalCellStyle --

            #region -- TitleCellStyle --

            TitleCellStyle = cellFactory.CreateStyle();
            TitleCellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            TitleCellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            TitleCellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            TitleCellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            TitleCellStyle.Font.Size = 11;
            TitleCellStyle.Font.Name = fontName;
            TitleCellStyle.Font.IsBold = true;
            TitleCellStyle.HorizontalAlignment = TextAlignmentType.Left;
            TitleCellStyle.VerticalAlignment = TextAlignmentType.Center;
            TitleCellStyle.IsTextWrapped = needIsTextWrapped;

            #endregion -- TitleCellStyle --

            #region -- TextWrappedStyle --

            TextWrappedStyle = cellFactory.CreateStyle();
            TextWrappedStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            TextWrappedStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            TextWrappedStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            TextWrappedStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            TextWrappedStyle.Font.Size = 11;
            TextWrappedStyle.Font.Name = fontName;
            TextWrappedStyle.Font.IsBold = false;
            TextWrappedStyle.HorizontalAlignment = TextAlignmentType.Left;
            TextWrappedStyle.VerticalAlignment = TextAlignmentType.Center;
            TextWrappedStyle.IsTextWrapped = needIsTextWrapped;

            #endregion -- TextWrappedStyle --

            #region -- HighLightStyle --

            HighLightStyle = cellFactory.CreateStyle();
            //EditableCellStyle.ForegroundColor = System.Drawing.Color.FromArgb(198, 234, 206);
            HighLightStyle.ForegroundColor = System.Drawing.Color.FromArgb(255, 242, 204);
            HighLightStyle.Pattern = BackgroundType.Solid;
            HighLightStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            HighLightStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            HighLightStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            HighLightStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            HighLightStyle.Font.Size = 11;
            HighLightStyle.Font.Name = fontName;
            HighLightStyle.Font.IsBold = false;
            HighLightStyle.HorizontalAlignment = TextAlignmentType.Left;
            HighLightStyle.VerticalAlignment = TextAlignmentType.Center;
            HighLightStyle.IsTextWrapped = needIsTextWrapped;

            #endregion -- NormalCellStyle --

            #region -- NoteCellStyle --

            NoteCellStyle = cellFactory.CreateStyle();
            NoteCellStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.None;
            NoteCellStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.None;
            NoteCellStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.None;
            NoteCellStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.None;
            NoteCellStyle.Font.Size = 11;
            NoteCellStyle.Font.Name = fontName;
            NoteCellStyle.Font.IsBold = false;
            NoteCellStyle.Font.Color = System.Drawing.Color.Gray;
            NoteCellStyle.HorizontalAlignment = TextAlignmentType.Left;
            NoteCellStyle.VerticalAlignment = TextAlignmentType.Center;
            NoteCellStyle.IsTextWrapped = needIsTextWrapped;

            #endregion -- NoteCellStyle --
        }
    }

    public enum ExcelLanguageType
    {
        English = 0,
        Japanese = 1,
        German = 2,
        Chinese = 3,
        Dutch = 4
    }
}
