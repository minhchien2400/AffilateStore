﻿using AffiliateStoreBE.Common.Models;
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
using AffiliateStoreBE.Service;

namespace AffiliateStoreBE.Common.Service
{
    public class ExcelHelper
    {
        public static List<T> ReadExcel<T>(Stream stream, string sheetName, int firstDataIndex = 1) where T : ExcelDynamic
        {
            var result = new List<T>();
            var datas = new List<Dictionary<string, string>>();
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

                    datas.Add(cellValueMapping);
                }
            }
            return result;
        }
        private List<T> UpdateValue<T>(ImportType type, List<Dictionary<string, string>> excelDatas)
        {
            if(type == ImportType.ImportProducts)
            {
                var product = new ProductSheetModel();
                var products = new List<ProductSheetModel>();
                foreach(var item in excelDatas)
                {
                    product.ProductName = item["Name"];
                    product.Description = item["Description"];
                    product.Price = int.Parse(item["Price"]);
                    product.CategoryName = item["Category name"];
                    products.Add(product);
                }
                return products;
            }
            else if (type == ImportType.ImportImages)
            {
                var image = new ImageSheetModel();
                var images = new List<ImageSheetModel>();
                foreach(var item in excelDatas)
                {
                    image.ProductName = item["Product name"];
                    image.Image = item["Image link"];
                    images.Add(image);
                }
            }
        }
    }
}
