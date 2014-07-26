using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;


namespace Smartpoint.Utilities
{
    public class ExcelReader
    {
        public ExcelReader()
        {

        }

        public List<List<string>> RetrieveRowsCollection(Stream file)
        {
            List<List<string>> rowList = null;
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(file, false))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                string cellValue = string.Empty;

                rowList = GetRowsData(workbookPart, sheetData, 0, 0);

            }

            return rowList;
        }

        private List<List<string>> GetRowsData(WorkbookPart workbookPart, SheetData sheetData, int rowCount, int skip)
        {
            List<List<string>> dataRows = new List<List<string>>();
            List<string> dataRow = null;
            string cellValue = string.Empty;

            IEnumerable<Row> rows = null;
            if (rowCount > 0)
            {
                rows = sheetData.Elements<Row>().Skip(skip).Take(rowCount);
            }
            else
            {
                rows = sheetData.Elements<Row>().Skip(skip);
            }

            foreach (Row row in rows)
            {
                dataRow = new List<string>();
                foreach (Cell cell in row.Elements<Cell>())
                {

                    if (cell.DataType != null)
                    {
                        if (cell.DataType == CellValues.SharedString)
                        {
                            int id = -1;

                            if (Int32.TryParse(cell.InnerText, out id))
                            {
                                SharedStringItem item = GetSharedStringItemById(workbookPart, id);

                                if (item.Text != null)
                                {
                                    cellValue = item.Text.Text;
                                }
                                else if (item.InnerText != null)
                                {
                                    cellValue = item.InnerText;
                                }
                                else if (item.InnerXml != null)
                                {
                                    cellValue = item.InnerXml;
                                }
                            }
                        }
                    }
                    else
                    {
                        cellValue = cell.InnerText;
                    }

                    dataRow.Add(cellValue);
                }

                dataRows.Add(dataRow);
            }

            return dataRows;
        }

        public SharedStringItem GetSharedStringItemById(WorkbookPart workbookPart, int id)
        {
            return workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
        }


    }
}
