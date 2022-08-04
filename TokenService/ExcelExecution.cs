using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenService
{
    public class ExcelExecution
    {
        public void ExportExcel(string csr, string fileName, string countCSR)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("ToolResult");
            sheet1.SetColumnWidth(0, 1000);
            sheet1.SetColumnWidth(1, 12000);
            sheet1.SetColumnWidth(2, 12000);
            sheet1.SetColumnWidth(3, 12000);

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("STT");
            row1.CreateCell(1).SetCellValue("CSR");
            row1.CreateCell(2).SetCellValue("Certificate");
            row1.CreateCell(3).SetCellValue("Cert Chain");

            //Số lượng CSR cần sinh
            int Num = Int32.Parse(countCSR);
            int num = 4;
            for (int i = 1; i <= Num; i++)
            {
                IRow row2 = sheet1.CreateRow(i);
                row2.CreateCell(0).SetCellValue(i);
                row2.CreateCell(1).SetCellValue("" + csr + "");     //SetCellValue("\"" + csr + "\"")
                row2.CreateCell(2).SetCellValue("");
                row2.CreateCell(3).SetCellValue("");

            }

            FileStream sw = File.Create(fileName);    //file/ExportExcelCSR.xlsx
            workbook.Write(sw);
            sw.Close();
        }

        public string ImportExcel(string fileName, int cellColumn)
        {
            IWorkbook workbook;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            workbook = new XSSFWorkbook(fs);

            ISheet sheet = workbook.GetSheetAt(0);

            //SINGLE
            IRow curRow = sheet.GetRow(1);
            string cellValue = curRow.GetCell(cellColumn).StringCellValue.Trim();
            Console.WriteLine(cellValue);

            return cellValue;

            //MULTIPLE
            //for (int i = 2; i <= rowCount; i++)
            //{
            //    IRow curRow = sheet.GetRow(i);
            //    if (curRow == null)
            //    {
            //        rowCount = i - 1;
            //        break;
            //    }
            //    // Get data from the 4th column (4th cell of each row)
            //    var cellValue = curRow.GetCell(3).StringCellValue.Trim();
            //    Console.WriteLine(cellValue);
            //}
        }

    }
}
