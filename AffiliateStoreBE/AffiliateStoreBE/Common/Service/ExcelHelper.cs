using AffiliateStoreBE.Common.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel;
using System.Text;
using System.Xml;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Cells;
using Workbook = Aspose.Cells.Workbook;
using Worksheet = Aspose.Cells.Worksheet;
using Cell = Aspose.Cells.Cell;

namespace AffiliateStoreBE.Common.Service
{
    public class ExcelHelper
    {
        /// <summary>
        /// 此方法兼容Excel中间有空行的情况
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="sheetName"></param>
        /// <param name="datetimeFormat"></param>
        /// <param name="firstDataIndex"></param>
        /// <returns></returns>
        public static List<T> ReadExcel<T>(Stream stream, string sheetName, List<string> dynamicColumnNames = null, string datetimeFormat = "", int firstDataIndex = 1) where T : ExcelDynamic
        {
            var result = new List<T>();
            if (firstDataIndex < 1)
            {
                return result;
            }
            stream.Seek(0, SeekOrigin.Begin);
            using (Workbook workbook = new Workbook())
            {
                workbook.Copy(new Workbook(stream));
                Worksheet worksheet = null;
                if (string.IsNullOrEmpty(sheetName))
                {
                    worksheet = workbook.Worksheets[0];
                }
                else
                {
                    worksheet = workbook.Worksheets[sheetName];
                }
                if (worksheet == null)
                {
                    return null;
                }
                var columnIndexMapping = new Dictionary<int, string>();
                var cells = worksheet.Cells;
                var maxRowCount = cells.MaxDataRow;
                var maxColumnCount = cells.MaxDataColumn;

                #region -- get cell index - name mapping --

                for (var columnIndex = 0; columnIndex <= maxColumnCount; columnIndex++)
                {
                    var columnName = cells[firstDataIndex - 1, columnIndex].StringValue.TrimStart('*');
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        columnIndexMapping.Add(columnIndex, columnName);
                    }
                }

                #endregion -- get cell index - name mapping --

                for (var rowIndex = firstDataIndex; rowIndex <= maxRowCount; rowIndex++)
                {
                    var allEmpty = true;
                    var cellValueMapping = new Dictionary<string, string>();

                    #region -- get cell name - value mapping --

                    for (var columnIndex = 0; columnIndex <= maxColumnCount; columnIndex++)
                    {
                        if (columnIndexMapping.ContainsKey(columnIndex))
                        {
                            var cellName = columnIndexMapping[columnIndex].TrimStart('*');
                            var cellValue = cells[rowIndex, columnIndex].StringValue + "^~`" + cells[rowIndex, columnIndex].HtmlString;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                allEmpty = false;
                            }
                            if (!cellValueMapping.ContainsKey(cellName))
                            {
                                cellValueMapping.Add(cellName, cellValue);
                            }
                        }
                    }

                    #endregion -- get cell name - value mapping --

                    if (!allEmpty)
                    {
                        var errorMessage = new StringBuilder();
                        var readerObject = Activator.CreateInstance<T>();
                        readerObject.RowIndex = rowIndex;
                        var properties = readerObject.GetType().GetProperties();
                        foreach (var property in properties)
                        {
                            var defaultPropertyName = string.Empty;
                            var propertys = property.GetCustomAttributes(false).OfType<ExcelColumn>().FirstOrDefault();
                            var propertyNames = propertys?.Names;
                            if (propertyNames != null && propertyNames.Count > 0)
                            {
                                defaultPropertyName = propertyNames.FirstOrDefault(i => cellValueMapping.ContainsKey(I18NEntity.GetString(i)));
                                defaultPropertyName = !string.IsNullOrEmpty(defaultPropertyName) ? I18NEntity.GetString(defaultPropertyName) : String.Empty;
                            }
                            else if (property.PropertyType.ToString() == "System.Collections.Generic.List`1[AvePoint.Confucius.FeatureCommon.Domain.Excel.DynamicExcelModel]")
                            {
                                defaultPropertyName = "Dynamic";
                            }
                            if (!string.IsNullOrEmpty(defaultPropertyName))
                            {
                                #region -- set property values --

                                if (defaultPropertyName == "Dynamic")
                                {
                                    var dynamicExcelModelList = new List<DynamicExcelModel>();
                                    foreach (KeyValuePair<string, string> kvp in cellValueMapping)
                                    {
                                        if (dynamicColumnNames.Contains(kvp.Key.ToLower()))
                                        {
                                            var dynamicExcelModel = new DynamicExcelModel();
                                            dynamicExcelModel.Name = kvp.Key;
                                            dynamicExcelModel.Value = kvp.Value.Split("^~`")[0]?.Trim(' ');
                                            dynamicExcelModelList.Add(dynamicExcelModel);
                                        }
                                    }
                                    property.SetValue(readerObject, dynamicExcelModelList);
                                    continue;
                                }
                                var pValue = cellValueMapping[defaultPropertyName];
                                var pType = property.PropertyType;
                                var pUseHtml = propertys.UseHtml;
                                if (pUseHtml)
                                {
                                    var value = pValue.Split("^~`")[1];
                                    value = ReplaceRichContentSpaceAndStyle(value);
                                    pValue = value;
                                }
                                else
                                {
                                    var value = pValue.Split("^~`")[0];
                                    pValue = value;
                                }
                                if (pType.ToString() == "System.Int32")
                                {
                                    if (Int32.TryParse(pValue, out var intValue))
                                    {
                                        property.SetValue(readerObject, intValue);
                                    }
                                    else
                                    {
                                        readerObject.ReportStatus = ImportStatus.Failed;
                                        errorMessage.AppendLine(I18NEntity.GetString("AS_ImportCommon_InvalidInt_Message", defaultPropertyName));
                                    }
                                }
                                else if (pType.ToString() == "System.Boolean")
                                {
                                    var boolValue = BooleanHelper.ConvertToBoolean(pValue);
                                    if (boolValue.HasValue)
                                    {
                                        property.SetValue(readerObject, boolValue.Value);
                                    }
                                    else
                                    {
                                        readerObject.ReportStatus = ImportStatus.Failed;
                                        errorMessage.AppendLine(I18NEntity.GetString("AS_ImportCommon_InvalidBoolean_Message", defaultPropertyName));
                                    }
                                }
                                else if (pType.ToString() == "System.Guid")
                                {
                                    if (Guid.TryParse(pValue, out var guidValue))
                                    {
                                        property.SetValue(readerObject, guidValue);
                                    }
                                    else
                                    {
                                        readerObject.ReportStatus = ImportStatus.Failed;
                                        errorMessage.AppendLine(I18NEntity.GetString("AS_ImportCommon_InvalidGuid_Message", defaultPropertyName));
                                    }
                                }
                                else if (pType.ToString() == "System.DateTime")
                                {
                                    //if (DateTimeExtentions.TryToConvertDateTime(pValue, out var datetimeValue))
                                    //{
                                    //    property.SetValue(readerObject, datetimeValue);
                                    //}
                                    //else if (DateTimeExtentions.TryToConvertDateTimeUseDateString(pValue, out var dateValue))
                                    //{
                                    //    property.SetValue(readerObject, dateValue);
                                    //}
                                    //else
                                    //{
                                    //    readerObject.ReportStatus = ImportStatus.Failed;
                                    //    errorMessage.AppendLine(I18NEntity.GetString("AS_ImportCommon_InvalidDateTime_Message", defaultPropertyName));
                                    //}
                                }
                                else if (pType.ToString() == "System.String")
                                {
                                    if (string.IsNullOrEmpty(pValue))
                                    {
                                        property.SetValue(readerObject, pValue);
                                    }
                                    else
                                    {
                                        property.SetValue(readerObject, pValue.Trim());
                                    }
                                }
                                else if (pType.IsEnum)
                                {
                                    var isDescription = false;
                                    var fields = pType.GetFields();
                                    foreach (var field in fields)
                                    {
                                        var curDesc = (DescriptionAttribute[])field?.GetCustomAttributes(typeof(DescriptionAttribute), false);
                                        if (curDesc != null && curDesc.Length > 0)
                                        {
                                            if (curDesc[0].Description == pValue)
                                            {
                                                property.SetValue(readerObject, field.GetValue(null));
                                                isDescription = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!isDescription)
                                    {
                                        var o = Activator.CreateInstance(pType);
                                        if (int.TryParse(pValue, out var intValue))
                                        {
                                            var isEnumValue = false;
                                            foreach (var field in fields)
                                            {
                                                var fieldValue = field.GetValue(o);
                                                if ((int)fieldValue == intValue)
                                                {
                                                    isEnumValue = true;
                                                    property.SetValue(readerObject, fieldValue);
                                                    break;
                                                }
                                            }
                                            if (!isEnumValue)
                                            {
                                                //invalid enum
                                                readerObject.ReportStatus = ImportStatus.Failed;
                                                errorMessage.AppendLine(I18NEntity.GetString("AS_ImportCommon_InvalidEnum_Message", defaultPropertyName));
                                            }
                                        }
                                        else
                                        {
                                            readerObject.ReportStatus = ImportStatus.Failed;
                                            errorMessage.AppendLine(I18NEntity.GetString("AS_ImportCommon_InvalidEnum_Message", defaultPropertyName));
                                        }
                                    }
                                }
                                else
                                {
                                    property.SetValue(readerObject, pValue);
                                }

                                #endregion -- set property values --
                            }
                        }
                        result.Add(readerObject);
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 此方法兼容Excel中间有空行的情况，但需要配合使用ReadExcelEx方法才生效。
        /// 同时增加只更新指定列内容的功能，见参数attributes的使用方法。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="worksheet"></param>
        /// <param name="sheetData"></param>
        /// <param name="firstDataIndex"></param>
        /// <param name="attributes">Specified attribute to update. If the attributes is null, all columns will be update.</param>

        public static MemoryStream CreateExcel<T>(List<T> data, string templateFile, int firstDataIndex = 1) where T : class
        {
            var stream = new MemoryStream();
            using (Workbook workbook = new Workbook(templateFile))
            {
                var worksheet = workbook.Worksheets[0];
                UpdateWorksheet(worksheet, data, firstDataIndex);
                workbook.Save(stream, SaveFormat.Xlsx);
            }
            return stream;
        }

        public static MemoryStream CreateExcel<T>(List<T> data, string templateFile, String sheetName, ExcelLanguageType language, int firstDataIndex = 1, List<CustomExcelItem> excelItems = null) where T : class
        {
            var stream = new MemoryStream();
            using (Workbook workbook = new Workbook(templateFile))
            {
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = sheetName;
                UpdateWorksheet(worksheet, data, language, firstDataIndex, false, excelItems);
                workbook.Save(stream, SaveFormat.Xlsx);
            }
            return stream;
        }

        public static void UpdateWorksheet<T>(Worksheet worksheet, List<T> sheetData, int firstDataIndex = 1, bool updateResultOnly = false) where T : class
        {
            var styleHelper = new StyleUtil();
            var cells = worksheet.Cells;

            var columnIndexs = new Dictionary<string, int>();
            var titleIndex = firstDataIndex - 1;
            if (titleIndex < 0)
            {
                return;
            }

            #region -- init columns --

            var j = 0;
            while (true)
            {
                var columnName = cells[titleIndex, j].StringValue.TrimStart('*');
                if (string.IsNullOrEmpty(columnName))
                {
                    break;
                }
                else
                {
                    if (!columnIndexs.ContainsKey(columnName))
                    {
                        columnIndexs.Add(columnName, j);
                    }
                }
                j++;
            }
            if (updateResultOnly)
            {
                if (!columnIndexs.ContainsKey("FeatureCommon_Export_Common_ImportStatus_Entry"))
                {
                    cells[titleIndex, j].PutValue(("FeatureCommon_Export_Common_ImportStatus_Entry"));
                    cells[titleIndex, j].SetStyle(styleHelper.TitleCellStyle);
                    columnIndexs.Add(("FeatureCommon_Export_Common_ImportStatus_Entry"), j);
                    j++;
                }
                if (!columnIndexs.ContainsKey(("FeatureCommon_Export_Common_Comments_Entry")))
                {
                    cells[titleIndex, j].PutValue(("FeatureCommon_Export_Common_Comments_Entry"));
                    cells[titleIndex, j].SetStyle(styleHelper.TitleCellStyle);
                    cells.SetColumnWidth(j, 50);
                    columnIndexs.Add(("FeatureCommon_Export_Common_Comments_Entry"), j);
                }
            }

            #endregion -- init columns --

            var hiddenList = new List<int>();
            var normalList = new List<int>();
            if (sheetData.Count > 0)
            {
                var properties = sheetData[0].GetType().GetProperties();
                foreach (var property in properties)
                {
                    var excelColumn = property.GetCustomAttributes(false).OfType<ExcelColumn>().FirstOrDefault();
                    var column = string.Empty;
                    var columnNames = excelColumn?.Names;
                    if (columnNames != null && columnNames.Count > 0)
                    {
                        column = columnNames.FirstOrDefault(c => columnIndexs.ContainsKey((c)));
                        column = !string.IsNullOrEmpty(column) ? (column) : String.Empty;
                    }
                    var canBeEdit = excelColumn?.Editable;
                    var isHidden = excelColumn?.Hidden;
                    var isTextWrapped = excelColumn?.TextWrapped;
                    if (!string.IsNullOrEmpty(column) && columnIndexs.ContainsKey(column))
                    {
                        var columnIndex = columnIndexs[column];
                        if (isHidden.HasValue && isHidden.Value)
                        {
                            hiddenList.Add(columnIndex);
                        }
                        if (updateResultOnly && !column.Equals(("FeatureCommon_Export_Common_ImportStatus_Entry"), StringComparison.OrdinalIgnoreCase) && !column.Equals(("FeatureCommon_Export_Common_Comments_Entry"), StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        for (var i = 0; i < sheetData.Count; i++)
                        {
                            var cellName = CellsHelper.CellIndexToName(i + firstDataIndex, columnIndex);
                            Cell cell = worksheet.Cells[cellName];
                            if (canBeEdit.HasValue && canBeEdit.Value)
                            {
                                cell.SetStyle(styleHelper.EditableCellStyle);
                            }
                            else if (isTextWrapped.HasValue && isTextWrapped.Value)
                            {
                                cell.SetStyle(styleHelper.TextWrappedStyle);
                            }
                            else
                            {
                                cell.SetStyle(styleHelper.NormalCellStyle);
                                normalList.Add(columnIndex);
                            }
                            var value = property.GetValue(sheetData[i]);
                            if (!property.PropertyType.IsEnum)
                            {
                                if (property.PropertyType.ToString() == "System.Boolean")
                                {
                                    cell.Value = BooleanHelper.ConvertToBooleanString(value);
                                }
                                else
                                {
                                    cell.Value = value;
                                }
                            }
                            else
                            {
                                var fieldInfo = value.GetType().GetField(value.ToString());
                                var attributes = (DescriptionAttribute[])fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false);
                                if (attributes != null && attributes.Length > 0)
                                {
                                    cell.Value = (attributes[0].Description);
                                }
                                else
                                {
                                    cell.Value = value;
                                }
                            }
                        }
                    }
                }
            }
            //normalList.Distinct().ToList().ForEach(i => worksheet.AutoFitColumn(i));//auto fit
            hiddenList.Distinct().ToList().ForEach(i => worksheet.Cells.HideColumn(i));//hidden
            worksheet.AutoFitRows();
        }

        public static Dictionary<string, int> UpdateWorksheet<T>(Worksheet worksheet, List<T> sheetData, ExcelLanguageType language, int firstDataIndex = 1, bool updateResultOnly = false, List<CustomExcelItem> excelItems = null, Dictionary<string, int> columnIndexs = null, bool autoFitRows = true) where T : class
        {
            var styleHelper = new StyleUtil(language);
            var cells = worksheet.Cells;
            if (columnIndexs == null)
            {
                columnIndexs = new Dictionary<string, int>();
            }

            var titleIndex = firstDataIndex - 1;
            if (titleIndex < 0)
            {
                return null;
            }

            #region -- init columns --

            var j = 0;
            while (true)
            {
                var columnName = cells[titleIndex, j].StringValue.TrimStart('*');
                if (string.IsNullOrEmpty(columnName))
                {
                    break;
                }
                else
                {
                    if (!columnIndexs.ContainsKey(columnName))
                    {
                        columnIndexs.Add(columnName, j);
                    }
                }
                j++;
            }
            if (updateResultOnly)
            {
                if (!columnIndexs.ContainsKey(("FeatureCommon_Export_Common_ImportStatus_Entry")))
                {
                    cells[titleIndex, j].PutValue(("FeatureCommon_Export_Common_ImportStatus_Entry"));
                    cells[titleIndex, j].SetStyle(styleHelper.TitleCellStyle);
                    columnIndexs.Add(("FeatureCommon_Export_Common_ImportStatus_Entry"), j);
                    j++;
                }
                if (!columnIndexs.ContainsKey(("FeatureCommon_Export_Common_Comments_Entry")))
                {
                    cells[titleIndex, j].PutValue(("FeatureCommon_Export_Common_Comments_Entry"));
                    cells[titleIndex, j].SetStyle(styleHelper.TitleCellStyle);
                    cells.SetColumnWidth(j, 50);
                    columnIndexs.Add(("FeatureCommon_Export_Common_Comments_Entry"), j);
                }
            }

            #endregion -- init columns --

            var hiddenList = new List<int>();
            var normalList = new List<int>();
            if (sheetData.Count > 0)
            {
                var properties = sheetData[0].GetType().GetProperties();
                foreach (var property in properties)
                {
                    var excelColumn = property.GetCustomAttributes(false).OfType<ExcelColumn>().FirstOrDefault();
                    var column = string.Empty;
                    var columnNames = excelColumn?.Names;
                    if (columnNames != null && columnNames.Count > 0)
                    {
                        column = columnNames.FirstOrDefault(c => columnIndexs.ContainsKey((c)));
                        column = !string.IsNullOrEmpty(column) ? (column) : String.Empty;
                    }
                    var canBeEdit = excelColumn?.Editable;
                    var isHidden = excelColumn?.Hidden;
                    var isTextWrapped = excelColumn?.TextWrapped;
                    var text = excelColumn?.Text;
                    var noteName = excelColumn?.NoteName;
                    if (!string.IsNullOrEmpty(column) && columnIndexs.ContainsKey(column))
                    {
                        var columnIndex = columnIndexs[column];
                        if (isHidden.HasValue && isHidden.Value)
                        {
                            hiddenList.Add(columnIndex);
                        }
                        if (updateResultOnly && !column.Equals(("FeatureCommon_Export_Common_ImportStatus_Entry"), StringComparison.OrdinalIgnoreCase) && !column.Equals(("FeatureCommon_Export_Common_Comments_Entry"), StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        for (var i = 0; i < sheetData.Count; i++)
                        {
                            bool isNoCover = false;
                            var cellName = CellsHelper.CellIndexToName(i + firstDataIndex, columnIndex);
                            Cell cell = worksheet.Cells[cellName];
                            if (canBeEdit.HasValue && canBeEdit.Value)
                            {
                                if (text.HasValue && text.Value)
                                {
                                    styleHelper.EditableCellStyle.Number = 49;
                                }
                                cell.SetStyle(styleHelper.EditableCellStyle);
                                isNoCover = true;
                            }
                            else if (isTextWrapped.HasValue && isTextWrapped.Value)
                            {
                                if (text.HasValue && text.Value)
                                {
                                    styleHelper.TextWrappedStyle.Number = 49;
                                }
                                cell.SetStyle(styleHelper.TextWrappedStyle);
                            }
                            else
                            {
                                if (text.HasValue && text.Value)
                                {
                                    styleHelper.NormalCellStyle.Number = 49;
                                }
                                cell.SetStyle(styleHelper.NormalCellStyle);
                                normalList.Add(columnIndex);
                            }
                            var value = property.GetValue(sheetData[i]);
                            if (!property.PropertyType.IsEnum)
                            {
                                if (property.PropertyType.ToString() == "System.Boolean")
                                {
                                    cell.Value = BooleanHelper.ConvertToBooleanString(value);
                                }
                                else
                                {
                                    if (value != null && value.ToString().StartsWith("##HYPERLINK##"))
                                    {
                                        worksheet.Hyperlinks.Add(i + firstDataIndex, columnIndex, 1, 1, value.ToString().Replace("##HYPERLINK##", string.Empty));
                                    }
                                    else if (value != null && value.ToString().Length > 32000)
                                    {
                                        cell.Value = value.ToString().Substring(0, 32000);
                                    }
                                    else
                                    {
                                        cell.Value = value;
                                    }
                                }
                            }
                            else
                            {
                                var fieldInfo = value.GetType().GetField(value.ToString());
                                var attributes = (DescriptionAttribute[])fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false);
                                if (attributes != null && attributes.Length > 0)
                                {
                                    cell.Value = (attributes[0].Description);
                                }
                                else
                                {
                                    cell.Value = value;
                                }
                            }
                            if (!string.IsNullOrEmpty(noteName))
                            {
                                var noteProperty = properties.FirstOrDefault(p => p.Name.EqualsIgnoreCase(noteName));
                                var noteValue = noteProperty?.GetValue(sheetData[i])?.ToString();
                                if (!string.IsNullOrEmpty(noteValue))
                                {
                                    var commt = worksheet.Comments[worksheet.Comments.Add(cellName)];
                                    commt.Note = GetFormatComment(noteValue, language);
                                    commt.AutoSize = true;
                                    //Font font = commt.Font;
                                    //font.IsBold = true;
                                    //commt.FormatCharacters(0, 9, font, new StyleFlag());
                                }
                            }
                            var ishightPropertie = properties.FirstOrDefault(p => p.Name.EqualsIgnoreCase("hasFlagHighLight"));
                            if (ishightPropertie?.PropertyType.ToString() == "System.Boolean")
                            {
                                bool.TryParse(ishightPropertie?.GetValue(sheetData[i])?.ToString(), out var ishightPValue);
                                if (ishightPValue && !isNoCover)
                                {
                                    styleHelper.HighLightStyle.ForegroundColor = System.Drawing.Color.FromArgb(255, 242, 204);
                                    cell.SetStyle(styleHelper.HighLightStyle);
                                }
                            }
                            if (column.Equals(("AM_Marking_Variance_Column_Entry")))
                            {
                                var varianceHighLight = properties.FirstOrDefault(p => p.Name.EqualsIgnoreCase("varianceHighLight"));
                                if (varianceHighLight?.PropertyType.ToString() == "System.Boolean")
                                {
                                    bool.TryParse(varianceHighLight?.GetValue(sheetData[i])?.ToString(), out var ishightPValue);
                                    if (ishightPValue && !isNoCover)
                                    {
                                        styleHelper.HighLightStyle.ForegroundColor = System.Drawing.Color.FromArgb(248, 223, 220);
                                        cell.SetStyle(styleHelper.HighLightStyle);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (excelItems != null && excelItems.Count > 0)
            {
                foreach (var excelItem in excelItems)
                {

                    var cellName = CellsHelper.CellIndexToName(excelItem.Row, excelItem.Column);
                    Cell cell = worksheet.Cells[cellName];
                    cell.Value = excelItem.Value;
                    if (excelItem.ExcelItemStyle == ExcelItemStyle.ExcelNotes)
                    {
                        cell.SetStyle(styleHelper.NoteCellStyle);
                    }
                    if (excelItem.ExcelItemStyle == ExcelItemStyle.VarianceHighLight)
                    {
                        styleHelper.HighLightStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.None;
                        styleHelper.HighLightStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.None;
                        styleHelper.HighLightStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.None;
                        styleHelper.HighLightStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.None;
                        styleHelper.HighLightStyle.ForegroundColor = System.Drawing.Color.FromArgb(248, 223, 220);
                        cell.SetStyle(styleHelper.HighLightStyle);
                    }
                    if (excelItem.ExcelItemStyle == ExcelItemStyle.FlagHighLight)
                    {
                        styleHelper.HighLightStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.None;
                        styleHelper.HighLightStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.None;
                        styleHelper.HighLightStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.None;
                        styleHelper.HighLightStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.None;
                        styleHelper.HighLightStyle.ForegroundColor = System.Drawing.Color.FromArgb(255, 242, 204);
                        cell.SetStyle(styleHelper.HighLightStyle);
                    }
                }
            }

            //normalList.Distinct().ToList().ForEach(i => worksheet.AutoFitColumn(i));//auto fit
            hiddenList.Distinct().ToList().ForEach(i => worksheet.Cells.HideColumn(i));//hidden
            if (autoFitRows)
            {
                worksheet.AutoFitRows();
            }
            return columnIndexs;
        }

        public static void UpdateWorksheetWithReport<T>(Worksheet worksheet, List<T> sheetData, int firstDataIndex = 1) where T : class
        {
            var styleHelper = new StyleUtil();
            var cells = worksheet.Cells;
            var columnIndexs = new Dictionary<string, int>();
            var titleIndex = firstDataIndex - 1;
            if (titleIndex < 0)
            {
                return;
            }

            #region -- init columns --

            var j = 0;
            while (true)
            {
                var columnName = cells[titleIndex, j].StringValue.TrimStart('*');
                if (string.IsNullOrEmpty(columnName))
                {
                    break;
                }
                else
                {
                    if (!columnIndexs.ContainsKey(columnName))
                    {
                        columnIndexs.Add(columnName, j);
                    }
                }
                j++;
            }
            //if (updateResultOnly)
            //{
            if (!columnIndexs.ContainsKey(("FeatureCommon_Export_Common_ImportStatus_Entry")))
            {
                cells[titleIndex, j].PutValue(("FeatureCommon_Export_Common_ImportStatus_Entry"));
                cells[titleIndex, j].SetStyle(styleHelper.TitleCellStyle);
                columnIndexs.Add(("FeatureCommon_Export_Common_ImportStatus_Entry"), j);
                j++;
            }
            if (!columnIndexs.ContainsKey(("FeatureCommon_Export_Common_Comments_Entry")))
            {
                cells[titleIndex, j].PutValue(("FeatureCommon_Export_Common_Comments_Entry"));
                cells[titleIndex, j].SetStyle(styleHelper.TitleCellStyle);
                cells.SetColumnWidth(j, 50);
                columnIndexs.Add(("FeatureCommon_Export_Common_Comments_Entry"), j);
            }
            //}

            #endregion -- init columns --

            var hiddenList = new List<int>();
            var normalList = new List<int>();
            if (sheetData.Count > 0)
            {
                var properties = sheetData[0].GetType().GetProperties();
                foreach (var property in properties)
                {
                    var excelColumn = property.GetCustomAttributes(false).OfType<ExcelColumn>().FirstOrDefault();
                    var column = string.Empty;
                    var columnNames = excelColumn?.Names;
                    if (columnNames != null && columnNames.Count > 0)
                    {
                        column = columnNames.FirstOrDefault(c => columnIndexs.ContainsKey((c)));
                        column = !string.IsNullOrEmpty(column) ? (column) : String.Empty;
                    }
                    var canBeEdit = excelColumn?.Editable;
                    var isHidden = excelColumn?.Hidden;
                    var isTextWrapped = excelColumn?.TextWrapped;
                    if (!string.IsNullOrEmpty(column) && columnIndexs.ContainsKey(column))
                    {
                        var columnIndex = columnIndexs[column];
                        if (isHidden.HasValue && isHidden.Value)
                        {
                            hiddenList.Add(columnIndex);
                        }
                        //if (updateResultOnly && !column.Equals(ReportStatusColumnName, StringComparison.OrdinalIgnoreCase) && !column.Equals(ReportMessageColumnName, StringComparison.OrdinalIgnoreCase))
                        //{
                        //    continue;
                        //}
                        for (var i = 0; i < sheetData.Count; i++)
                        {
                            var cellName = CellsHelper.CellIndexToName(i + firstDataIndex, columnIndex);
                            Cell cell = worksheet.Cells[cellName];
                            if (canBeEdit.HasValue && canBeEdit.Value)
                            {
                                cell.SetStyle(styleHelper.EditableCellStyle);
                            }
                            else if (isTextWrapped.HasValue && isTextWrapped.Value)
                            {
                                cell.SetStyle(styleHelper.TextWrappedStyle);
                            }
                            else
                            {
                                cell.SetStyle(styleHelper.NormalCellStyle);
                                normalList.Add(columnIndex);
                            }
                            var value = property.GetValue(sheetData[i]);
                            if (!property.PropertyType.IsEnum)
                            {
                                if (property.PropertyType.ToString() == "System.Boolean")
                                {
                                    cell.Value = BooleanHelper.ConvertToBooleanString(value);
                                }
                                else
                                {
                                    cell.Value = value;
                                }
                            }
                            else
                            {
                                var fieldInfo = value.GetType().GetField(value.ToString());
                                var attributes = (DescriptionAttribute[])fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false);
                                if (attributes != null && attributes.Length > 0)
                                {
                                    cell.Value = (attributes[0].Description);
                                }
                                else
                                {
                                    cell.Value = value;
                                }
                            }
                        }
                    }
                }
            }

            var rowCount = worksheet.Cells.MaxDataRow;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                worksheet.Cells.SetRowHeight(rowIndex, 15);
            }

            //normalList.Distinct().ToList().ForEach(i => worksheet.AutoFitColumn(i));//auto fit
            hiddenList.Distinct().ToList().ForEach(i => worksheet.Cells.HideColumn(i));//hidden
            worksheet.AutoFitRows();
        }

        public static void UpdateWorksheetWithReport<T>(Worksheet worksheet, List<T> sheetData, ExcelLanguageType language, int firstDataIndex = 1) where T : class
        {
            var styleHelper = new StyleUtil(language);
            var cells = worksheet.Cells;
            var columnIndexs = new Dictionary<string, int>();
            var titleIndex = firstDataIndex - 1;
            if (titleIndex < 0)
            {
                return;
            }

            #region -- init columns --

            var j = 0;
            while (true)
            {
                var columnName = cells[titleIndex, j].StringValue.TrimStart('*');
                if (string.IsNullOrEmpty(columnName))
                {
                    break;
                }
                else
                {
                    if (!columnIndexs.ContainsKey(columnName))
                    {
                        columnIndexs.Add(columnName, j);
                    }
                }
                j++;
            }
            //if (updateResultOnly)
            //{
            if (!columnIndexs.ContainsKey(("FeatureCommon_Export_Common_ImportStatus_Entry")))
            {
                cells[titleIndex, j].PutValue(("FeatureCommon_Export_Common_ImportStatus_Entry"));
                cells[titleIndex, j].SetStyle(styleHelper.TitleCellStyle);
                cells.SetColumnWidth(j, 20);
                columnIndexs.Add(("FeatureCommon_Export_Common_ImportStatus_Entry"), j);
                j++;
            }
            if (!columnIndexs.ContainsKey(("FeatureCommon_Export_Common_Comments_Entry")))
            {
                cells[titleIndex, j].PutValue(("FeatureCommon_Export_Common_Comments_Entry"));
                cells[titleIndex, j].SetStyle(styleHelper.TitleCellStyle);
                cells.SetColumnWidth(j, 50);
                columnIndexs.Add(("FeatureCommon_Export_Common_Comments_Entry"), j);
            }
            //}

            #endregion -- init columns --

            var hiddenList = new List<int>();
            var normalList = new List<int>();
            if (sheetData.Count > 0)
            {
                var properties = sheetData[0].GetType().GetProperties();
                foreach (var property in properties)
                {
                    var excelColumn = property.GetCustomAttributes(false).OfType<ExcelColumn>().FirstOrDefault();
                    var column = string.Empty;
                    var columnNames = excelColumn?.Names;
                    if (columnNames != null && columnNames.Count > 0)
                    {
                        column = columnNames.FirstOrDefault(c => columnIndexs.ContainsKey((c)));
                        column = !string.IsNullOrEmpty(column) ? (column) : String.Empty;
                    }
                    var canBeEdit = excelColumn?.Editable;
                    var isHidden = excelColumn?.Hidden;
                    var isTextWrapped = excelColumn?.TextWrapped;
                    if (!string.IsNullOrEmpty(column) && columnIndexs.ContainsKey(column))
                    {
                        var columnIndex = columnIndexs[column];
                        if (isHidden.HasValue && isHidden.Value)
                        {
                            hiddenList.Add(columnIndex);
                        }
                        //if (updateResultOnly && !column.Equals(ReportStatusColumnName, StringComparison.OrdinalIgnoreCase) && !column.Equals(ReportMessageColumnName, StringComparison.OrdinalIgnoreCase))
                        //{
                        //    continue;
                        //}
                        for (var i = 0; i < sheetData.Count; i++)
                        {
                            bool isNoCover = false;
                            var cellName = CellsHelper.CellIndexToName(i + firstDataIndex, columnIndex);
                            Cell cell = worksheet.Cells[cellName];
                            if (canBeEdit.HasValue && canBeEdit.Value)
                            {
                                cell.SetStyle(styleHelper.EditableCellStyle);
                                isNoCover = true;
                            }
                            else if (isTextWrapped.HasValue && isTextWrapped.Value)
                            {
                                cell.SetStyle(styleHelper.TextWrappedStyle);
                            }
                            else
                            {
                                cell.SetStyle(styleHelper.NormalCellStyle);
                                normalList.Add(columnIndex);
                            }
                            var value = property.GetValue(sheetData[i]);
                            if (!property.PropertyType.IsEnum)
                            {
                                if (property.PropertyType.ToString() == "System.Boolean")
                                {
                                    cell.Value = BooleanHelper.ConvertToBooleanString(value);
                                }
                                else
                                {
                                    cell.Value = value;
                                }
                            }
                            else
                            {
                                var fieldInfo = value.GetType().GetField(value.ToString());
                                var attributes = (DescriptionAttribute[])fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false);
                                if (attributes != null && attributes.Length > 0)
                                {
                                    cell.Value = (attributes[0].Description);
                                }
                                else
                                {
                                    cell.Value = value;
                                }
                            }
                            var ishightPropertie = properties.FirstOrDefault(p => p.Name.EqualsIgnoreCase("hasFlagHighLight"));
                            if (ishightPropertie?.PropertyType.ToString() == "System.Boolean")
                            {
                                bool.TryParse(ishightPropertie?.GetValue(sheetData[i])?.ToString(), out var ishightPValue);
                                if (ishightPValue && !isNoCover)
                                {
                                    styleHelper.HighLightStyle.ForegroundColor = System.Drawing.Color.FromArgb(255, 242, 204);
                                    cell.SetStyle(styleHelper.HighLightStyle);
                                }
                            }
                            if (column.Equals(("AM_Marking_Variance_Column_Entry")))
                            {
                                var varianceHighLight = properties.FirstOrDefault(p => p.Name.EqualsIgnoreCase("varianceHighLight"));
                                if (varianceHighLight?.PropertyType.ToString() == "System.Boolean")
                                {
                                    bool.TryParse(varianceHighLight?.GetValue(sheetData[i])?.ToString(), out var ishightPValue);
                                    if (ishightPValue && !isNoCover)
                                    {
                                        styleHelper.HighLightStyle.ForegroundColor = System.Drawing.Color.FromArgb(248, 223, 220);
                                        cell.SetStyle(styleHelper.HighLightStyle);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var rowCount = worksheet.Cells.MaxDataRow;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                worksheet.Cells.SetRowHeight(rowIndex, 15);
            }

            //normalList.Distinct().ToList().ForEach(i => worksheet.AutoFitColumn(i));//auto fit
            hiddenList.Distinct().ToList().ForEach(i => worksheet.Cells.HideColumn(i));//hidden
            worksheet.AutoFitRows();
        }

        public static void UpdateWorksheetWithReportNoComment<T>(Worksheet worksheet, List<T> sheetData, int firstDataIndex = 1) where T : class
        {
            var styleHelper = new StyleUtil();
            var cells = worksheet.Cells;
            var columnIndexs = new Dictionary<string, int>();
            var titleIndex = firstDataIndex - 1;
            if (titleIndex < 0)
            {
                return;
            }

            #region -- init columns --

            var j = 0;
            while (true)
            {
                var columnName = cells[titleIndex, j].StringValue;
                if (string.IsNullOrEmpty(columnName))
                {
                    break;
                }
                else
                {
                    if (!columnIndexs.ContainsKey(columnName))
                    {
                        columnIndexs.Add(columnName, j);
                    }
                }
                j++;
            }
            //if (updateResultOnly)
            //{
            if (!columnIndexs.ContainsKey(CommonConstants.ReportStatusColumnName))
            {
                cells[titleIndex, j].PutValue(CommonConstants.ReportStatusColumnName);
                cells[titleIndex, j].SetStyle(styleHelper.TitleCellStyle);
                columnIndexs.Add(CommonConstants.ReportStatusColumnName, j);
                j++;
            }
            //}

            #endregion -- init columns --

            var hiddenList = new List<int>();
            var normalList = new List<int>();
            if (sheetData.Count > 0)
            {
                var properties = sheetData[0].GetType().GetProperties();
                foreach (var property in properties)
                {
                    var excelColumn = property.GetCustomAttributes(false).OfType<ExcelColumn>().FirstOrDefault();
                    var column = string.Empty;
                    var columnNames = excelColumn?.Names;
                    if (columnNames != null && columnNames.Count > 0)
                    {
                        column = columnNames.FirstOrDefault(c => columnIndexs.ContainsKey(c));
                    }
                    var canBeEdit = excelColumn?.Editable;
                    var isHidden = excelColumn?.Hidden;
                    var isTextWrapped = excelColumn?.TextWrapped;
                    if (!string.IsNullOrEmpty(column) && columnIndexs.ContainsKey(column))
                    {
                        var columnIndex = columnIndexs[column];
                        if (isHidden.HasValue && isHidden.Value)
                        {
                            hiddenList.Add(columnIndex);
                        }
                        //if (updateResultOnly && !column.Equals(ReportStatusColumnName, StringComparison.OrdinalIgnoreCase) && !column.Equals(ReportMessageColumnName, StringComparison.OrdinalIgnoreCase))
                        //{
                        //    continue;
                        //}
                        for (var i = 0; i < sheetData.Count; i++)
                        {
                            var cellName = CellsHelper.CellIndexToName(i + firstDataIndex, columnIndex);
                            Cell cell = worksheet.Cells[cellName];
                            if (canBeEdit.HasValue && canBeEdit.Value)
                            {
                                cell.SetStyle(styleHelper.EditableCellStyle);
                            }
                            else if (isTextWrapped.HasValue && isTextWrapped.Value)
                            {
                                cell.SetStyle(styleHelper.TextWrappedStyle);
                            }
                            else
                            {
                                cell.SetStyle(styleHelper.NormalCellStyle);
                                normalList.Add(columnIndex);
                            }
                            var value = property.GetValue(sheetData[i]);
                            if (!property.PropertyType.IsEnum)
                            {
                                if (property.PropertyType.ToString() == "System.Boolean")
                                {
                                    cell.Value = BooleanHelper.ConvertToBooleanString(value);
                                }
                                else
                                {
                                    cell.Value = value;
                                }
                            }
                            else
                            {
                                var fieldInfo = value.GetType().GetField(value.ToString());
                                var attributes = (DescriptionAttribute[])fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false);
                                if (attributes != null && attributes.Length > 0)
                                {
                                    cell.Value = (attributes[0].Description);
                                }
                                else
                                {
                                    cell.Value = value;
                                }
                            }
                        }
                    }
                }
            }

            var rowCount = worksheet.Cells.MaxDataRow;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                worksheet.Cells.SetRowHeight(rowIndex, 15);
            }

            normalList.Distinct().ToList().ForEach(i => worksheet.AutoFitColumn(i));//auto fit
            hiddenList.Distinct().ToList().ForEach(i => worksheet.Cells.HideColumn(i));//hidden
        }

        private static Dictionary<string, int> GetColumnIndexMapping(int firstDataIndex, Cells cells)
        {
            var mapping = new Dictionary<string, int>();
            var columnCount = cells.MaxDataColumn;
            for (var columnIndex = 0; columnIndex <= columnCount; columnIndex++)
            {
                var columnName = cells[firstDataIndex, columnIndex].StringValue;
                if (!string.IsNullOrEmpty(columnName) && !mapping.ContainsKey(columnName))
                {
                    mapping.Add(columnName, columnIndex);
                }
            }
            return mapping;
        }

        public static MemoryStream CreateMultiSheetExcel<T>(Dictionary<string, List<T>> data, string templateFile, int firstDataIndex = 1) where T : class
        {
            var stream = new MemoryStream();
            using (Workbook workbook = new Workbook(templateFile))
            {
                foreach (var sheetData in data)
                {
                    var worksheet = workbook.Worksheets[sheetData.Key];
                    if (worksheet != null)
                    {
                        UpdateWorksheet(worksheet, sheetData.Value, firstDataIndex);
                    }
                }
                workbook.Save(stream, SaveFormat.Xlsx);
            }
            return stream;
        }

        public static MemoryStream CreateExcelBySheetList(List<Worksheet> sheetList, int activeSheetIndex = 0)
        {
            var stream = new MemoryStream();
            using (Workbook workbook = new Workbook())
            {
                workbook.Worksheets.Clear();
                foreach (var sheet in sheetList)
                {
                    if (workbook.Worksheets[sheet.Name] == null)
                    {
                        var sheetIndex = workbook.Worksheets.Add();
                        workbook.Worksheets[sheetIndex].Name = sheet.Name;
                        workbook.Worksheets[sheetIndex].Copy(sheet);
                    }
                }
                workbook.Worksheets.ActiveSheetIndex = activeSheetIndex;
                workbook.Save(stream, SaveFormat.Xlsx);
            }
            return stream;
        }

        public static MemoryStream CreateExcel<T>(List<T> data, Stream templateStream, int firstDataIndex = 1) where T : class
        {
            var stream = new MemoryStream();
            using (Workbook workbook = new Workbook(templateStream))
            {
                //workbook.Copy(new Workbook(templateStream));
                var worksheet = workbook.Worksheets[0];
                UpdateWorksheet(worksheet, data, firstDataIndex, true);
                workbook.Save(stream, SaveFormat.Xlsx);
            }
            return stream;
        }

        //public static string CreateReport<T>(List<T> data, Stream importStream, ICacheService distributedCache, int firstDataIndex = 1) where T : class
        //{
        //    var reportName = Guid.NewGuid().ToString();
        //    using (var stream = CreateExcel(data, importStream, firstDataIndex))
        //    {
        //        distributedCache.AddBlob(reportName, stream.ToArray());
        //        //distributedCache.Add(reportName, stream.ToArray());
        //    }
        //    return reportName;
        //}


        public static MemoryStream CreateExcelWithHeaderIndex<T>(List<T> data, Stream templateStream, int firstDataIndex = 1, int headerIndex = 0) where T : class
        {
            var stream = new MemoryStream();
            using (Workbook workbook = new Workbook())
            {
                workbook.Copy(new Workbook(templateStream));
                var worksheet = workbook.Worksheets[0];
                var cells = worksheet.Cells;

                var columnIndexs = new Dictionary<string, int>();
                var j = 0;
                while (true)
                {
                    var columnName = cells[headerIndex, j].StringValue;
                    if (string.IsNullOrEmpty(columnName))
                    {
                        break;
                    }
                    else
                    {
                        if (!columnIndexs.ContainsKey(columnName))
                        {
                            columnIndexs.Add(columnName, j);
                        }
                    }
                    j++;
                }
                var headerStyle = cells[headerIndex, 0].GetStyle(false);
                if (!columnIndexs.ContainsKey(("FeatureCommon_Export_Common_ImportStatus_Entry")))
                {
                    cells[headerIndex, j].PutValue(("FeatureCommon_Export_Common_ImportStatus_Entry"));
                    cells[headerIndex, j].SetStyle(headerStyle);
                    columnIndexs.Add(("FeatureCommon_Export_Common_ImportStatus_Entry"), j);
                    j++;
                }
                if (!columnIndexs.ContainsKey(("FeatureCommon_Export_Common_Comments_Entry")))
                {
                    cells[headerIndex, j].PutValue(("FeatureCommon_Export_Common_Comments_Entry"));
                    cells[headerIndex, j].SetStyle(headerStyle);
                    columnIndexs.Add(("FeatureCommon_Export_Common_Comments_Entry"), j);
                }
                for (var i = 0; i < data.Count; i++)
                {
                    var properties = data[i].GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        var column = property.GetCustomAttributes(false).OfType<ExcelColumn>().FirstOrDefault()?.Name;
                        if (!string.IsNullOrEmpty(column) && columnIndexs.ContainsKey(column))
                        {
                            var columnIndex = columnIndexs[column];
                            var cellName = CellsHelper.CellIndexToName(i + firstDataIndex, columnIndex);
                            Cell cell = worksheet.Cells[cellName];
                            cell.Value = property.GetValue(data[i]);
                        }
                    }
                }
                workbook.Save(stream, SaveFormat.Xlsx);
            }
            return stream;
        }

        public static string ReplaceRichContentSpaceAndStyle(string richText)
        {
            var htmlDoc = new HtmlDocument
            {
                OptionOutputAsXml = true,
                OptionOutputOriginalCase = true
            };
            htmlDoc.LoadHtml(richText);
            ReplaceSpace(htmlDoc.DocumentNode);
            var result = CommonUtil.ConvertHtmlDocumentToString(htmlDoc);
            return string.IsNullOrEmpty(CommonUtil.GetInnerText(result).Trim()) ? "" : CommonUtil.RepalceRichText(result);
        }

        private static void ReplaceSpace(HtmlNode node)
        {
            if (node != null && node.ChildNodes.Any())
            {
                for (var i = 0; i < node.ChildNodes.Count; i++)
                {
                    var item = node.ChildNodes[i];
                    if (item.NodeType == HtmlNodeType.Text)
                    {
                        var textNode = (HtmlTextNode)item;
                        if (textNode.Text.StartsWith(" "))
                        {
                            textNode.Text = "&nbsp;" + item.InnerText.Remove(0, 1);
                        }
                        if (item.InnerText.EndsWith(" "))
                        {
                            textNode.Text = item.InnerText.Remove(item.InnerText.Length - 1, 1) + "&nbsp;";
                        }
                        textNode.Text = textNode.Text.Replace("  ", "&nbsp; ").Replace("  ", "&nbsp; ");
                    }
                    else if (item.ChildNodes.Any())
                    {
                        ReplaceSpace(item);
                    }
                }
            }
        }

        private static string GetFormatComment(string content, ExcelLanguageType wordlanguage)
        {
            var resultStr = new StringBuilder();
            if (new List<string>() { "\r\n", "\n", "\r" }.Any(l => content.Contains(l)) || content.Length <= 30)
            {
                resultStr.Append(content);
            }
            else if (wordlanguage == ExcelLanguageType.Japanese && !content.Contains(' '))
            {
                var index = 1;
                foreach (var c in content)
                {
                    if (index % 30 == 0)
                    {
                        resultStr.Append("\r\n");
                    }
                    resultStr.Append(c);
                    index++;
                }
            }
            else
            {
                var wordList = content.Split(' ');
                var currentLine = "";
                for (var i = 0; i < wordList.Count(); i++)
                {
                    var currentWord = wordList[i];
                    var line = currentLine == "" ? currentWord : currentLine + " " + currentWord;
                    if (line.Length <= 30)
                    {
                        currentLine = line;
                    }
                    else
                    {
                        resultStr.Append(currentLine + "\r\n");
                        currentLine = currentWord;
                    }
                }
                resultStr.Append(currentLine);
            }
            return resultStr.ToString().Trim();
        }
    }
}
