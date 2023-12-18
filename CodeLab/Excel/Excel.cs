using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Excel
{
    public class Excel
    {
        public string Path { get; set; }
        public DataTable dt { get; set; }
        public Excel(string path)
        {
            Path = path;

            dt = ReadExcelToDataTable(path) ?? new DataTable();
        }

        public DataTable? ReadExcelToDataTable(string filePath)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet? worksheet = package.Workbook.Worksheets.FirstOrDefault(p=>p.Name == "9.10月");

                if (worksheet == null) return null;

                DataTable dataTable = new DataTable(worksheet.Name);

                // 添加表格的列
                foreach (var firstRowCell in worksheet.Cells[3, 1, 1, 10])
                {
                    dataTable.Columns.Add(firstRowCell.Text);
                }

                // 添加表格的資料
                for (int rowNumber = 4; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var worksheetRow = worksheet.Cells[rowNumber, 1, rowNumber, 10];
                    DataRow row = dataTable.Rows.Add();
                    foreach (var cell in worksheetRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }

                return dataTable;
            }
        }
    }
}


