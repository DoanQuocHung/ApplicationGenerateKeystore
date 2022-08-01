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
    public class WriteExcelFile
    {
        public void ExportExcel(string csr,string cipher)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("ToolResult");
            sheet1.SetColumnWidth(0, 1000);
            sheet1.SetColumnWidth(1, 12000);
            sheet1.SetColumnWidth(2, 12000);
            sheet1.SetColumnWidth(3, 12000);
            sheet1.SetColumnWidth(4, 8000);

            IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("STT");
            row1.CreateCell(1).SetCellValue("Csr");
            row1.CreateCell(2).SetCellValue("Certificate");
            row1.CreateCell(3).SetCellValue("Cert Chain");
            row1.CreateCell(4).SetCellValue("Cipher");

            //Số lượng CSR cần sinh
            int num = 4;
            for(int i = 1; i <= num; i++)
            {
                IRow row2 = sheet1.CreateRow(i);
                row2.CreateCell(0).SetCellValue(i);
                row2.CreateCell(1).SetCellValue("" + csr + "");     //SetCellValue("\"" + csr + "\"")
                row2.CreateCell(2).SetCellValue("");
                row2.CreateCell(3).SetCellValue("");
                row2.CreateCell(4).SetCellValue(cipher);
            }

            FileStream sw = File.Create("file/ExportExcelCSR.xlsx");
            workbook.Write(sw);
            sw.Close();
        }

        public void ImportExcel()
        {

        }
    }
}
